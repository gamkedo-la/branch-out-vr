using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Leaf : TreeLimbBase
{
    float percentageToEndBranch;

    private void OnEnable()
    {
        EnergyTickTimer.Tick += Photosynthesis;
    }

    //Initialize if spawned from a branch
    public void Initialize(UnityEvent growEvent, TertiaryBranch previousTertiaryBranch)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTertiaryBranch;
        if (previousTertiaryBranch != null)
        {
            transform.localEulerAngles = GetRandomRotations();
        }
        Initialize();

    } 

    /// <summary>
    /// Every leaf node will create energy for the tree every Tick.
    /// </summary>
    private void Photosynthesis()
    {
        TreeTest.Instance.CreateEnergy(energySystemValues.leafEnergyGainPerTick); 
    }

    void Initialize()
    {
        //tertiaryBranchPrefab = limbContainer.tertiaryBranch;
        //percentageToEndBranch = Random.value;
    }

    public override void Grow()
    {
        base.Grow();
    }

    private void OnDisable()
    {
        EnergyTickTimer.Tick -= Photosynthesis;
    }

}
