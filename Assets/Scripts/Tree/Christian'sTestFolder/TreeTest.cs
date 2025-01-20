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

    public EnergyPathNode rootNode; //Root of the tree, for the energy particles path
    private List<Transform> globalPathPoints = new();

    [SerializeField]
    float progress = 0;


    public UnityEvent GrowthHappenedEvent;

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
        rootNode.AddChild(trunkTest.pathNode);

        //currentTotalEnergy = 10000f;
        //trunkTest.Energy = 10000f;
        currentFreeEnergy = 0;
        UpdateGlobalPath();

    }

    public void UpdateGlobalPath()
    {
        globalPathPoints = rootNode.GetPathPoints();
        UpdateParticlesPath(globalPathPoints);
    }

    private void UpdateParticlesPath(List<Transform> pathPoints)
    {
        energyParticlesManager.SetPath(pathPoints);   
    }

    public void CreateEnergy(float amount)
    {
        currentTotalEnergy += amount;
        currentFreeEnergy += amount;
    }

    public void RemoveEnergy(float amount)
    {
        currentTotalEnergy -= amount;
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

        currentTotalEnergy += 1;

        if (currentFreeEnergy >= 0)
        {
            trunkTest.Energy += 1;
            RemoveEnergy(1f);
        }

        GrowthHappenedEvent?.Invoke();
    }
}
