using System.Collections.Generic;
using System.Linq;
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
    private bool pathUpdate = true;

    private struct ParticleOnPath
    {
        public ParticleSystem.Particle particle;
        public int currentPathIndex;
    };

    private ParticleOnPath[] particlesOnPath;

    private InputAction toggleView;

    private void Start()
    {
        toggleView = PlayerInputManager.Instance.inputActions.FindAction("ToggleEnergyView");

        if (toggleView != null)
        {
            toggleView.performed += _ => ToggleEnergy();
        }
        transform.position = tree.transform.position;
        particles = new ParticleSystem.Particle[1000];
        pathUpdate = true;
        particlesOnPath = new ParticleOnPath[particles.Length];
    }

    void ToggleEnergy()
    {
        Debug.Log("toggle energy");
        energyViewActive = !energyViewActive;

        if (energyViewActive)
        {
            energyParticleSystem.Emit(particles.Length);
            if (!pathUpdate)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].position = transform.InverseTransformPoint(GetCurrentTarget(i).position);
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
        pathUpdate = true;
        // Debug.Log(path.Count + " path points");
    }

    private int GetStartPathIndex(int particleIndex)
    {
        int pathIndex = particleIndex % path.Count;
        particlesOnPath[particleIndex].particle = particles[particleIndex];
        particlesOnPath[particleIndex].currentPathIndex = pathIndex;
        return pathIndex;

    }

    private Transform GetCurrentTarget(int currentParticleIndex)
    {
        Transform target;
        if (pathUpdate)
        {
            target = path[GetStartPathIndex(currentParticleIndex)];
        }
        else
        {
            target = path[particlesOnPath[currentParticleIndex].currentPathIndex];
        }

        if (Vector3.Distance(particles[currentParticleIndex].position, transform.InverseTransformPoint(target.position)) <= 0.2f)
        {
            int newIndex = path.IndexOf(target) + 1;
            if (newIndex >= path.Count)
            {
                newIndex = 0;
            }
            particlesOnPath[currentParticleIndex].currentPathIndex = newIndex;
            Transform newTarget = path[newIndex];

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
                    Transform target = GetCurrentTarget(i);
                    particles[i].position = Vector3.MoveTowards(particles[i].position, transform.InverseTransformPoint(target.position), Time.deltaTime * flowSpeed);
                }
                if (pathUpdate)
                {
                    pathUpdate = false;
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
