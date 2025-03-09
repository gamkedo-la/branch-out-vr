using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnergyParticlesFlow : MonoBehaviour
{
    [SerializeField] private ProceduralTree tree;
    [SerializeField] private float flowSpeed = 2f;

    public bool energyViewActive = false; // Is the energy particle system active and visible? 
    public ParticleSystem energyParticleSystem; // The particle system for the energy
    public List<Transform> path; // The list of path points the energy particles follow around the tree

    private ParticleSystem.Particle[] particles; // The particles of our particle system
    /// <summary>
    /// This tracks whether the list of path points has been updated. When the path changes, the particles need to update their target path points to include the added points, or exclude removed ones.
    /// </summary>
    private bool pathUpdate = true; 

    /// <summary>
    /// A struct for a single particle that's following a path that holds a reference to the particle and the current index of the path point it's moving towards.
    /// </summary>
    private struct ParticleOnPath
    {
        public ParticleSystem.Particle particle;
        public int currentPathIndex;
    };

    private ParticleOnPath[] particlesOnPath; // An array of all of the particles in our particle system and the index they are on in the list of path points.

    private InputAction toggleView; // Input action that toggles the energy particles on/off

    private bool win = false;

    private void Start()
    {
        toggleView = PlayerInputManager.Instance.inputActions.FindAction("ToggleEnergyView");

        // Subscribe to the input action if it's not null
        if (toggleView != null)
        {
            toggleView.performed += ToggleEnergyPlayerInput;
        }
        transform.position = tree.transform.position; // Zero the particle systems position to the tree's position

        // Default particle count is 1000, but TODO: update the count so that it reflects how the tree is doing; low particle count if it's dying,
        // high count if it's doing well
        particles = new ParticleSystem.Particle[1000]; 
        pathUpdate = true;
        particlesOnPath = new ParticleOnPath[particles.Length];
        ProceduralTree.OnGameWin += OnWinGame;
        GardenSceneUI.OnResumeGame += OnContinuePlayingAfterWin;
    }
    private void OnWinGame()
    {
        win = true;
        ToggleEnergy(true);
        ProceduralTree.OnGameWin -= OnWinGame;
    }

    private void OnContinuePlayingAfterWin()
    {
        ToggleEnergy(false);
    }
    private void ToggleEnergy(bool? explicitlySetState = null)
    {
        Debug.Log("toggle energy");
        if (explicitlySetState != null)
        {
            energyViewActive = explicitlySetState.Value;
        }
        else
        {
            energyViewActive = !energyViewActive; // Toggle the state of energy particles dynamically
        }

        if (energyViewActive)
        {
            energyParticleSystem.Emit(particles.Length); // Need to call Emit() to actually create the visible particles
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
            energyParticleSystem.Clear(); // Clear the list of particles when it's inactive
        }
    }
    void ToggleEnergyPlayerInput(InputAction.CallbackContext context)
    {
        ToggleEnergy();
    }
    /// <summary>
    /// Update the list of path points for the energy particle system.
    /// </summary>
    /// <param name="pathPoints"></param>
    public void SetPath(List<Transform> pathPoints)
    {
        path = pathPoints;
        pathUpdate = true; // Since the path has just been updated, set pathUpdate to true so the particles can be assigned to the new points.
        // Debug.Log(path.Count + " path points");
    }
    /// <summary>
    /// When there is a change to the path points, we update all the particles current target path point index to include new ones or exclude removed ones.
    /// </summary>
    /// <param name="particleIndex"></param>
    /// <returns></returns>
    private int GetStartPathIndex(int particleIndex)
    {
        int pathIndex = particleIndex % path.Count;
        particlesOnPath[particleIndex].particle = particles[particleIndex];
        particlesOnPath[particleIndex].currentPathIndex = pathIndex;
        return pathIndex;

    }
    /// <summary>
    /// Gets the current target path point for a particle given the particles index.
    /// </summary>
    /// <param name="currentParticleIndex"></param>
    /// <returns></returns>
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

    private void LateUpdate()
    {
        if (energyViewActive) // Only update the particles movement if our energy view is active
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

    private void OnDestroy()
    {
        if (toggleView != null)
        {
            toggleView.performed -= ToggleEnergyPlayerInput;
        }
        ProceduralTree.OnGameWin -= OnWinGame;
        GardenSceneUI.OnResumeGame -= OnContinuePlayingAfterWin;
    }
}
