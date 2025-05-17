using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProceduralTree : MonoBehaviour
{
    [SerializeField] EnergyParticlesFlow energyParticlesManager;
    public LimbContainer limbContainer;
    public Trunk trunkTest;
    public float growthTime = 2f;
    public float startingEnergy = 500f;
    public float currentTotalEnergy = 1800f;
    public float currentFreeEnergy = 1800f;
    public HashSet<TreeLimbBase> growingLimbs = new();
    public int numPotentialGrowthLocations = 0;

    public EnergyPathNode rootNode; //Root of the tree, for the energy particles path
    private List<Transform> globalPathPoints = new();

    [SerializeField]
    float progress = 0;

    public UnityEvent GrowthHappenedEvent;

    public static event Action OnGameOver;
    public static event Action OnGameWin;

    public bool win = false;

    public float userCutBranchCount = 0;

    private bool pauseTreeSystem = false;

    private void Awake()
    {
        GardenSceneUI.OnPauseGame += SetTreePaused;
        GardenSceneUI.OnResumeGame += ResumeTreeSystem;

        if (rootNode == null)
        {
            rootNode = GetComponent<EnergyPathNode>();
            rootNode.root = true;
        }
    }

    void Start()
    {
        currentTotalEnergy = startingEnergy;
        win = false;
        //create the first node of the trunk and initialize it
        trunkTest = Instantiate(limbContainer.trunk, transform, false);
        trunkTest.Initialize(GrowthHappenedEvent);
        rootNode.AddChild(trunkTest.nodes[0].pathNode); //Set this object as the root of the path that the energy particles follow, and update list of global path points
        trunkTest.nodes[0].pathNode.parent = rootNode;
        UpdateGlobalPath();
    }

    private void SetTreePaused()
    {
        pauseTreeSystem = true;
    }

    private void ResumeTreeSystem()
    {
        pauseTreeSystem = false;
    }
    /// <summary>
    /// Called when new path points are added or removed, this method updates the global list of path points (EnergyPathNode) for the energy system particles.
    /// </summary>
    public void UpdateGlobalPath()
    {
        if (rootNode != null)
        {
            globalPathPoints = rootNode.GetPathPoints();
            UpdateParticlesPath(globalPathPoints);
        }
    }

    /// <summary>
    /// This method provides the energy particle system with most up-to-date list of global path points.
    /// </summary>
    /// <param name="pathPoints"></param>
    private void UpdateParticlesPath(List<Transform> pathPoints)
    {
        energyParticlesManager.SetPath(pathPoints);   
    }

    /// <summary>
    /// Called when energy is generated (currently only when leaves grow/photosynthesize) or consumed and updates the tree's total energy.
    /// <param name="adjustFreeEnergyToo">Adjust the value of currentFreeEnergy by the same amount.</param>
    public void UpdateEnergy(float amount, bool adjustFreeEnergyToo = false)
    {
        if (!pauseTreeSystem)
        {
            currentTotalEnergy += amount;
            if (adjustFreeEnergyToo)
            {
                Debug.Log("Adjust free energy too.");
                currentFreeEnergy += amount;
            }


            if (!win)
            {
                if (currentTotalEnergy <= 0)
                {
                    AudioManager.Instance.PlaySFX("SFX_Tree_Dying");
                    GrowthHappenedEvent.RemoveAllListeners();
                    OnGameOver?.Invoke();
                }

                if (currentTotalEnergy >= Mathf.Floor(startingEnergy + startingEnergy / 5))
                {
                    win = true;
                    SetTreePaused();
                    OnGameWin?.Invoke();
                }
            }

        }
    }

    /// <summary>
    /// Used to allocate/pool energy in preparation for use, ie. when pooling energy for new growth. If meant to spend and remove energy (such as when actively growing), use SpendAndRemoveEnergy() instead.
    /// </summary>
    /// <param name="amount"></param>
    public void AllocateEnergy(float amount)
    {
        currentFreeEnergy -= amount;

        if (currentFreeEnergy <= 0 )
        {
            Debug.LogWarning("Attempted to allocate more free energy than was available; please check logic and ensure limbs cannot use free energy that doesn't exist!");
        }
    }

    public void ReleaseAllocatedEnergy(float amount)
    {
        currentFreeEnergy += amount;
    }

    public void StressReaction()
    {
        //TODO: need to determine and code criteria for tree becoming stressed and not putting any energy towards growth, change energy visualization colors if extreme
        //losing x percentage of energy within short span of time
        //watered too frequently
    }

    void Update()
    {
        //timer for when the growth will happen
        if (!pauseTreeSystem)
        {
            progress += Time.deltaTime;


            if (progress > growthTime)
                Grow();
        }
    }

    //Reset timer and raise event for all nodes in the tree to update
    public void Grow()
    {
        progress = 0;

        if (currentFreeEnergy > 0 && currentTotalEnergy > 0)
        {
            float energy = 1 * (userCutBranchCount / 3f);
            trunkTest.Energy += energy > 2 ? energy : 2;
        }

        if (Application.isPlaying)
        {
            GrowthHappenedEvent?.Invoke();
        }
    }

    private void OnDisable()
    {
        GardenSceneUI.OnPauseGame -= SetTreePaused;
        GardenSceneUI.OnResumeGame -= ResumeTreeSystem;
        GrowthHappenedEvent.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        GardenSceneUI.OnPauseGame -= SetTreePaused;
        GardenSceneUI.OnResumeGame -= ResumeTreeSystem;
        GrowthHappenedEvent.RemoveAllListeners();
    }
}
