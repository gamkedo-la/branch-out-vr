using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

public class SecondaryBranch : TreeLimbBase
{
    [SerializeField] TertiaryBranch tertiaryBranchPrefab;
    SecondaryBranch secondaryBranchPrefab;

    private bool isLimbTerminated = false;
    public BranchNode parentNode;

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
    //Initialize if spawned from a trunk
    public void Initialize(UnityEvent growEvent, SecondaryBranch previousTrunk, TreeTest tree)
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
    void Initialize()
    {
        tertiaryBranchPrefab = limbContainer.tertiaryBranch;
        secondaryBranchPrefab = limbContainer.secondaryBranch;
    }

    public override void AddChild()
    {
        base.AddChild();

        BranchNode parentNode = GetClosestNodeToBranch();

        TreeLimbBase limb = Instantiate(tertiaryBranchPrefab, GetRandomPositionOnLimb(), Quaternion.Euler(GetRandomBranchRotation()), parentNode.transform);
        branchedLimbs.Add(limb);
        EnergyPathNode energyPath = parentNode.gameObject.GetComponent<EnergyPathNode>();
        energyPath.AddChild(limb.nodes[0].GetComponent<EnergyPathNode>());
        (limb as TertiaryBranch).Initialize(GrowthHappenedEvent, this, thisTree);
        //when switched to BranchNode growing child, add logic for Bone0 EnergyPathNode to have this node as parent for calculating path
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
            TreeLimbBase limb = Instantiate(secondaryBranchPrefab, top.position, top.rotation, transform);
            nextLimb = (limb);
            (limb as SecondaryBranch).Initialize(GrowthHappenedEvent, this, thisTree);
            isLimbTerminated = LimbTerminated();
        }
    }
}
