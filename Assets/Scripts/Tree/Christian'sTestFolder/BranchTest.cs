using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BranchTest : TreeLimbBase
{
    public SecondaryBranch secondaryBranchPrefab;
    BranchTest branchPrefab;

    float percentageToEndBranch;

    //Initialize if spawned from a trunk
    public void Initialize(UnityEvent growEvent, TrunkTest previousTrunk, TreeTest tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTrunk;
        SetThisTree(tree);
        if (previousTrunk != null)
        {
            transform.localEulerAngles = GetRandomRotations();
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
        if (previousBranch != null)
        {
            transform.localEulerAngles = GetRandomRotations();
        }
        thisTree.UpdateGlobalPath();
        Initialize();

    }
    void Initialize()
    {
        branchPrefab = limbContainer.branchTest;
        secondaryBranchPrefab = limbContainer.secondaryBranch;
    }

    public override void AddChild()
    {
        base.AddChild();
        TreeLimbBase limb = Instantiate(secondaryBranchPrefab, GetRandomPositionOnLimb(), Quaternion.Euler(GetRandomRotations()), transform);
        branchedLimbs.Add(limb);
        (limb as SecondaryBranch).Initialize(GrowthHappenedEvent, this, thisTree);
        //when switched to BranchNode growing child, add logic for Bone0 EnergyPathNode to have this node as parent for calculating path
    }

    public override void Grow()
    {
        base.Grow();

        if (!IsMature)
            return;

        if(LimbTerminated()) 
            return;

        //TODO: need to adjust so that nextLimb grabs new growth point before it grows; also, newly grown branch must become a child of the BranchNode object it grows off of in order 
        //to have all branches removed when the parent node is cut
        if (nextLimb == null)
        {
            TreeLimbBase limb = Instantiate(branchPrefab, top.position, top.rotation, transform);
            nextLimb = (limb);
            (limb as BranchTest).Initialize(GrowthHappenedEvent, this, thisTree);
        }

    }
}
