using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TertiaryBranch : TreeLimbBase
{
    TertiaryBranch tertiaryBranchPrefab;
    Leaf leafPrefab;

    public void Initialize(UnityEvent growEvent, TertiaryBranch previousTertiaryBranch, ProceduralTree tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTertiaryBranch;
        SetThisTree(tree);
        thisTree.growingLimbs.Add(this);
        thisTree.numPotentialGrowthLocations--;
        if (previousTertiaryBranch != null)
        {
            transform.localEulerAngles = GetRandomBranchRotation();
        }
        thisTree.UpdateGlobalPath();
        Initialize();

    } 
    public void Initialize(UnityEvent growEvent, SecondaryBranch previousSecondaryBranch, ProceduralTree tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousSecondaryBranch;
        SetThisTree(tree);
        thisTree.growingLimbs.Add(this);
        thisTree.numPotentialGrowthLocations--;
        if (previousSecondaryBranch != null)
        {
            transform.localEulerAngles = GetRandomBranchRotation();
        }
        thisTree.UpdateGlobalPath();
        Initialize();

    }
    void Initialize()
    {
        tertiaryBranchPrefab = limbContainer.taperedTertiaryBranch;
        leafPrefab = limbContainer.leaf;
    }

    public override void AddChild()
    {
        base.AddChild();


        TreeLimbBase limb = Instantiate(leafPrefab, GetRandomPositionOnLimb(), Quaternion.Euler(GetRandomBranchRotation()), transform);
        branchedLimbs.Add(limb);

        (limb as Leaf).Initialize(GrowthHappenedEvent, this, thisTree);
        //when switched to BranchNode growing child, add logic for Bone0 EnergyPathNode to have this node as parent for calculating path
    }
    public override void Grow()
    {
        base.Grow();

        if (!IsMature)
            return;

        if (LimbTerminated())
            return;


        if (nextLimb == null)
        {
            TreeLimbBase limb = Instantiate(leafPrefab, top.position, Quaternion.Euler(GetRandomBranchRotation()), transform);
            nextLimb = (limb);
            (limb as Leaf).Initialize(GrowthHappenedEvent, this, thisTree);
        }

    }

}
