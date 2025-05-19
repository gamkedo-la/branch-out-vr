using UnityEngine;
using UnityEngine.Events;

public class SecondaryBranch : TreeLimbBase
{
    private TertiaryBranch taperedTertiaryBranchPrefab;
    private SecondaryBranch taperedSecondaryBranchPrefab;

    //Initialize if spawned from a branch
    public void Initialize(UnityEvent growEvent, Branch previousBranch, ProceduralTree tree)
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
    public void Initialize(UnityEvent growEvent, SecondaryBranch previousTrunk, ProceduralTree tree)
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
        taperedTertiaryBranchPrefab = limbContainer.taperedTertiaryBranch;
        taperedSecondaryBranchPrefab = limbContainer.taperedSecondaryBranch;
    }

    public override void AddChild()
    {
        base.AddChild();

        BranchNode parentNode = GetClosestNode(nextChildGrowPosition);

        TreeLimbBase limb = Instantiate(taperedTertiaryBranchPrefab, GetRandomPositionOnLimb(), Quaternion.Euler(GetRandomBranchRotation()), parentNode.transform);
        branchedLimbs.Add(limb);
        EnergyPathNode energyPath = parentNode.gameObject.GetComponent<EnergyPathNode>();
        energyPath.AddChild(limb.nodes[0].GetComponent<EnergyPathNode>());
        limb.nodes[0].pathNode.parent = energyPath;
        (limb as TertiaryBranch).Initialize(GrowthHappenedEvent, this, thisTree);
        //when switched to BranchNode growing child, add logic for Bone0 EnergyPathNode to have this node as parent for calculating path
    }

    public override void Grow()
    {
        base.Grow();

        if (!IsMature)
            return;

        if (CheckLimbTerminated())
            return;

        if (nextLimb == null)
        {
            TreeLimbBase limb = Instantiate(taperedSecondaryBranchPrefab, top.position, top.rotation, transform);
            nextLimb = (limb);
            (limb as SecondaryBranch).Initialize(GrowthHappenedEvent, this, thisTree);
        }
    }
}
