using UnityEngine;
using UnityEngine.Events;

public class TertiaryBranch : TreeLimbBase
{
    private TertiaryBranch taperedTertiaryBranchPrefab;
    private TertiaryBranch nonTaperedTertiaryBranchPrefab;
    private Leaf leafPrefab;

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
        LimbTerminated();
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
        LimbTerminated();
        thisTree.UpdateGlobalPath();
        Initialize();

    }
    void Initialize()
    {
        taperedTertiaryBranchPrefab = limbContainer.taperedTertiaryBranch;
        nonTaperedTertiaryBranchPrefab = limbContainer.nonTaperedTertiaryBranch;
        leafPrefab = limbContainer.leaf;
    }

    public override void AddChild()
    {
        base.AddChild();

        BranchNode parentNode = GetClosestNodeToBranch(nextChildGrowPosition);

        TreeLimbBase limb = 
            Instantiate(
                leafPrefab, 
                GetRandomPositionOnLimb(), 
                Quaternion.Euler(GetRandomBranchRotation()), 
                transform);

        branchedLimbs.Add(limb);
        EnergyPathNode energyPath = parentNode.gameObject.GetComponent<EnergyPathNode>();
        energyPath.AddChild(limb.nodes[0].GetComponent<EnergyPathNode>());
        limb.nodes[0].pathNode.parent = energyPath;
        (limb as Leaf).Initialize(GrowthHappenedEvent, this, thisTree);
        //when switched to BranchNode growing child, add logic for Bone0 EnergyPathNode to have this node as parent for calculating path

        nextChildGrowPosition = GetRandomPositionOnLimb();
        nextChildGrowRotation = GetRandomBranchRotation();
    }
    public override void Grow()
    {
        base.Grow();

        if (!IsMature) {return;}
        if (IsLimbTerminated) {return;}

        if (nextLimb == null)
        {
            // Last limb? Use tapered tertiary branch. No? Use the non-tapered tertiary branch.
            TreeLimbBase prefabToUse = 
                LimbTerminated() ? 
                taperedTertiaryBranchPrefab : 
                nonTaperedTertiaryBranchPrefab;

            TreeLimbBase limb = Instantiate(prefabToUse, top.position, top.rotation, transform);
            nextLimb = limb;
            (limb as TertiaryBranch).Initialize(GrowthHappenedEvent, this, thisTree);

            // TreeLimbBase limb = Instantiate(leafPrefab, top.position, Quaternion.Euler(GetRandomBranchRotation()), transform);
            // nextLimb = (limb);
            // (limb as Leaf).Initialize(GrowthHappenedEvent, this, thisTree);
        }
    }
}
