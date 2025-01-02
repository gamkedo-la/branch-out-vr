using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TreeTest : MonoBehaviour
{
    public static TreeTest Instance { get; private set; }
    public LimbContainer limbContainer;
    public TrunkTest trunkTest;
    public float growthTime = 2f;
    public float currentTotalEnergy = 10000f;
    public float currentFreeEnergy = 10000f;
     
    [SerializeField]
    float progress = 0;


    public UnityEvent GrowthHappenedEvent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        //create the first node of the trunk and initialize it
        trunkTest = Instantiate(limbContainer.trunkTest, transform, false);

        trunkTest.Initialize(GrowthHappenedEvent, null);
        currentTotalEnergy = 10000f;
        trunkTest.Energy = 10000f;
        currentFreeEnergy = 0;
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

        GrowthHappenedEvent?.Invoke();
    }
}
