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
    public void Initialize(UnityEvent growEvent, TertiaryBranch previousTertiaryBranch, TreeTest tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTertiaryBranch;
        SetThisTree(tree);
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
        if (thisTree != null)
        {
            thisTree.CreateEnergy(energySystemValues.leafEnergyGainPerTick);
        }
        else
        {
            Debug.LogWarning("Error: Leaf has no reference to its tree.");
        }
    }

    void Initialize()
    {
        //tertiaryBranchPrefab = limbContainer.tertiaryBranch;
        //percentageToEndBranch = Random.value;
    }

    public override void Grow()
    {
        //base.Grow();

        Photosynthesis();
    }

    private void OnDisable()
    {
        EnergyTickTimer.Tick -= Photosynthesis;
    }

}
