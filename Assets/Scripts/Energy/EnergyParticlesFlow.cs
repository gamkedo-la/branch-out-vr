using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnergyParticlesFlow : MonoBehaviour
{
    [SerializeField] private TreeTest tree;
    [SerializeField] private float flowSpeed = 2f;
    public ParticleSystem energyParticleSystem;
    public List<Transform> path;

    private ParticleSystem.Particle[] particles;
    private Transform[] nextPathPoints;
    private bool start = true;


    private void Start()
    {
        particles = new ParticleSystem.Particle[100];
        nextPathPoints = new Transform[particles.Length];
        energyParticleSystem.Emit(particles.Length);
        start = true;
    }

    public void SetPath(List<Transform> pathPoints)
    {
        path = pathPoints;
        Debug.Log(path.Count + " path points");
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
