using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TreeTest : MonoBehaviour
{
    [SerializeField] EnergyParticlesFlow energyParticlesManager;
    public LimbContainer limbContainer;
    public TrunkTest trunkTest;
    public float growthTime = 2f;
    public float currentTotalEnergy = 10000f;
    public float currentFreeEnergy = 10000f;
    public HashSet<TreeLimbBase> growingLimbs = new HashSet<TreeLimbBase>();
    public int numPotentialGrowthLocations = 0;

    public EnergyPathNode rootNode; //Root of the tree, for the energy particles path
    private List<Transform> globalPathPoints = new();

    [SerializeField]
    float progress = 0;

    public UnityEvent GrowthHappenedEvent;

    public static event Action OnGameOver;

    private void Awake()
    {
        if (rootNode == null)
        {
            rootNode = GetComponent<EnergyPathNode>();
            rootNode.root = true;
        }
    }

    void Start()
    {
        //create the first node of the trunk and initialize it
        trunkTest = Instantiate(limbContainer.trunkTest, transform, false);
        trunkTest.Initialize(GrowthHappenedEvent, null, this);
        rootNode.AddChild(trunkTest.pathNode); //Set this object as the root of the path that the energy particles follow, and update list of global path points
        UpdateGlobalPath();

    }

    /// <summary>
    /// Called when new path points are added or removed, this method updates the global list of path points (EnergyPathNode) for the energy system particles.
    /// </summary>
    public void UpdateGlobalPath()
    {
        globalPathPoints = rootNode.GetPathPoints();
        UpdateParticlesPath(globalPathPoints);
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
    /// Called when energy is generated (currently only when leaves grow/photosynthesize), or when a value is added to or subtracted from a limb's Energy property, and updates the tree's energy accordingly.
    /// </summary>
    /// <param name="amount">Use positive numbers to add to energy, negative to remove.</param>
    /// <param name="adjustFreeEnergyToo">Adjust the value of currentFreeEnergy by the same amount.</param>
    public void UpdateEnergy(float amount, bool adjustFreeEnergyToo = false)
    {
        currentTotalEnergy += amount;
        if (adjustFreeEnergyToo)
        {
            currentFreeEnergy += amount;
        }
        
        if (currentTotalEnergy <= 0)
        {
            OnGameOver?.Invoke();
        }
    }

    /// <summary>
    /// Used to remove energy from the total energy in the system; used when branches are cut. Use AllocateEnergy() if meant to allocate from free energy but not remove energy from the system.
    /// </summary>
    /// <param name="amount"></param>
    public void RemoveEnergy(float amount)
    {
        if (currentTotalEnergy <= 0)
        {
            Debug.LogWarning("Attempted to set total energy to a negative number; please check logic and ensure limbs cannot use energy that doesn't exist!");
        }

        currentTotalEnergy -= amount;
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
        progress += Time.deltaTime;

        if (progress > growthTime)
            Grow();
    }

    //Reset timer and raise event for all nodes in the tree to update
    public void Grow()
    {
        progress = 0;

        //currentTotalEnergy += 1;

        if (currentFreeEnergy > 0 && currentTotalEnergy > 0)
        {
            trunkTest.Energy += 1;
            UpdateEnergy(.25f, true);
        }

        GrowthHappenedEvent?.Invoke();
    }
}
