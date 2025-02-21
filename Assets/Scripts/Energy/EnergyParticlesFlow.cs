using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnergyParticlesFlow : MonoBehaviour
{
    [SerializeField] private TreeTest tree;
    [SerializeField] private float flowSpeed = 2f;

    [SerializeField] bool energyViewActive = false;
    public ParticleSystem energyParticleSystem;
    public List<Transform> path;

    private ParticleSystem.Particle[] particles;
    private Transform[] nextPathPoints;
    private bool start = true;

    private InputAction toggleView;

    private void Start()
    {
        toggleView = PlayerInputManager.Instance.inputActions.FindAction("ToggleEnergyView");

        if (toggleView != null)
        {
            toggleView.performed += _ => ToggleEnergy();
        }
        transform.position = tree.transform.position;
        particles = new ParticleSystem.Particle[100];
        nextPathPoints = new Transform[particles.Length];
        start = true;
    }

    void ToggleEnergy()
    {
        Debug.Log("toggle energy");
        energyViewActive = !energyViewActive;

        if (energyViewActive)
        {
            energyParticleSystem.Emit(particles.Length);
            if (!start)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].position = transform.InverseTransformPoint(GetCurrentTarget(particles[i], i).position);
                }
                energyParticleSystem.SetParticles(particles);
            }
        }
        else
        {
            Debug.Log("turn energy off");
            energyParticleSystem.Clear();
        }
    }

    public void SetPath(List<Transform> pathPoints)
    {
        path = pathPoints;
        // Debug.Log(path.Count + " path points");
    }

    private int GetStartPathIndex(int particleIndex)
    {
        return particleIndex % path.Count;

    }

    private Transform GetCurrentTarget(ParticleSystem.Particle currentParticle, int currentParticleIndex)
    {
        Transform target;
        if (start)
        {
            target = path[GetStartPathIndex(currentParticleIndex)];
            nextPathPoints[currentParticleIndex] = target;
        }
        else
        {
            if (nextPathPoints[currentParticleIndex] != null)
            {
                target = nextPathPoints[currentParticleIndex];
            }
            else
            {
                Debug.LogWarning($"Error - the nextPathPoints at index {currentParticleIndex} has a null value when not expected.");
                return path[0];
            }
        }

        if (Vector3.Distance(currentParticle.position, transform.InverseTransformPoint(target.position)) <= 0.2f)
        {
            int newIndex = path.IndexOf(target) + 1;
            if (newIndex >= path.Count)
            {
                newIndex = 0;
            }
            Transform newTarget = path[newIndex];
            nextPathPoints[currentParticleIndex] = newTarget;

            return newTarget;
        }
        return target;
    }

    private void Update()
    {
        if (energyViewActive)
        {
            int count = energyParticleSystem.GetParticles(particles);

            if (count > 0 && path.Count > 5)
            {
                for (int i = 0; i < count; i++)
                {
                    Transform target = GetCurrentTarget(particles[i], i);
                    particles[i].position = Vector3.MoveTowards(particles[i].position, transform.InverseTransformPoint(target.position), Time.deltaTime * flowSpeed);
                }
                if (start)
                {
                    start = false;
                }
                energyParticleSystem.SetParticles(particles);
            }
        }
    }

    private void OnDisable()
    {
        if (toggleView != null)
        {
            toggleView.performed -= _ => ToggleEnergy();
        }
    }
}
