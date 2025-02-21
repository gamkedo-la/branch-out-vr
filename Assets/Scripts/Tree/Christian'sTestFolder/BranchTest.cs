using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

public class BranchTest : TreeLimbBase
{
    public SecondaryBranch secondaryBranchPrefab;
    BranchTest branchPrefab;

    private bool isLimbTerminated = false;

    //Initialize if spawned from a trunk
    public void Initialize(UnityEvent growEvent, TrunkTest previousTrunk, TreeTest tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTrunk;
        SetThisTree(tree);
        thisTree.growingLimbs.Add(this);
        thisTree.numPotentialGrowthLocations--;
        if (previousTrunk != null)
        {
            transform.localEulerAngles = GetRandomBranchRotation();
        }
        thisTree.UpdateGlobalPath();
        Initialize();

    }
    //Initialize if spawned from a branch

    public void Initialize(UnityEvent growEvent, BranchTest previousBranch, TreeTest tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousBranch;
        SetThisTree(tree);
        thisTree.growingLimbs.Add(this);
        thisTree.numPotentialGrowthLocations--;
        if (previousBranch != null)
        {
            transform.localEulerAngles = GetRandomBranchRotation();
        }
        thisTree.UpdateGlobalPath();
        Initialize();

    }
    void Initialize()
    {
        branchPrefab = limbContainer.branchTest;
        secondaryBranchPrefab = limbContainer.secondaryBranch;

        nextChildGrowPosition = GetRandomPositionOnLimb();
        nextChildGrowRotation = GetRandomBranchRotation();

        Vector3[] boneRotations = HelperMethods.GetRandomRotationsForBones();

        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].transform.localRotation = Quaternion.Euler(boneRotations[i]);
        }
    }

    public override void AddChild()
    {
        base.AddChild();

        BranchNode parentNode = GetClosestNodeToBranch();

        TreeLimbBase limb = Instantiate(secondaryBranchPrefab, nextChildGrowPosition, Quaternion.Euler(nextChildGrowRotation), parentNode.transform);
        branchedLimbs.Add(limb);
        (limb as SecondaryBranch).Initialize(GrowthHappenedEvent, this, thisTree);

        nextChildGrowPosition = GetRandomPositionOnLimb();
        nextChildGrowRotation = GetRandomBranchRotation();
        //add logic for Bone0 EnergyPathNode to have this node as parent for calculating path
    }

    public override void Grow()
    {
        base.Grow();

        if (!IsMature)
            return;

        if (isLimbTerminated)
            return;

        if (nextLimb == null)
        {
            TreeLimbBase limb = Instantiate(branchPrefab, top.position, top.rotation, transform);
            nextLimb = (limb);
            (limb as BranchTest).Initialize(GrowthHappenedEvent, this, thisTree);
            isLimbTerminated = LimbTerminated();
        }

    }
}
