using UnityEngine;
using UnityEngine.Events;

public class TertiaryBranch : TreeLimbBase
{
    private TertiaryBranch taperedTertiaryBranchPrefab;
    //private TertiaryBranch nonTaperedTertiaryBranchPrefab;
    private Leaf leafPrefab;

    [SerializeField] private float terminateChance = 0.75f;
    protected override float TerminateChance => terminateChance;

    // initialize if spawned from a tertiary branch
    public void Initialize(UnityEvent growEvent, TertiaryBranch previousTertiaryBranch, ProceduralTree tree, bool isLastLimb)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTertiaryBranch;
        SetThisTree(tree);
        thisTree.growingLimbs.Add(this);
        if (previousTertiaryBranch != null)
        {
            transform.localEulerAngles = GetRandomBranchRotation();
        }

        IsLimbTerminated = isLastLimb;
        thisTree.UpdateGlobalPath();
        Initialize();

    } 

    // initialize if spawned from a secondary branch
    public void Initialize(UnityEvent growEvent, SecondaryBranch previousSecondaryBranch, ProceduralTree tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousSecondaryBranch;
        SetThisTree(tree);
        thisTree.growingLimbs.Add(this);
        if (previousSecondaryBranch != null)
        {
            transform.localEulerAngles = GetRandomBranchRotation();
        }
        thisTree.UpdateGlobalPath();
        Initialize();
    }
    void Initialize()
    {
        taperedTertiaryBranchPrefab = limbContainer.taperedTertiaryBranch;
        //nonTaperedTertiaryBranchPrefab = limbContainer.nonTaperedTertiaryBranch;
        leafPrefab = limbContainer.leaf;
    }

    public override void AddChild()
    {
        base.AddChild();

        BranchNode parentNode = GetClosestNode(nextChildGrowPosition);

        TreeLimbBase limb = 
            Instantiate(
                leafPrefab, 
                GetRandomPositionOnLimb(), 
                Quaternion.Euler(GetRandomBranchRotation()), 
                transform);

        branchedLimbs.Add(limb);
        (limb as Leaf).Initialize(GrowthHappenedEvent, this, thisTree);

        nextChildGrowPosition = GetRandomPositionOnLimb();
        nextChildGrowRotation = GetRandomBranchRotation();
    }
    public override void Grow()
    {
        base.Grow();

        if (!IsMature || nextLimb != null)
            return;

        // Determine if this is the final branch segment
        IsLimbTerminated = CheckLimbTerminated();

        if (nextLimb == null)
        {
            // Last limb? Use tapered tertiary branch. No? Use the non-tapered tertiary branch.
            /*            TreeLimbBase prefabToUse = 
                            IsLimbTerminated ? 
                            taperedTertiaryBranchPrefab : 
                            nonTaperedTertiaryBranchPrefab;*/

            // remove non-tapered branches for now
            TreeLimbBase prefabToUse = taperedTertiaryBranchPrefab;
            TreeLimbBase limb = Instantiate(prefabToUse, top.position, top.rotation, transform);
            nextLimb = limb;
            (limb as TertiaryBranch).Initialize(GrowthHappenedEvent, this, thisTree, IsLimbTerminated);

            // TreeLimbBase limb = Instantiate(leafPrefab, top.position, Quaternion.Euler(GetRandomBranchRotation()), transform);
            // nextLimb = (limb);
            // (limb as Leaf).Initialize(GrowthHappenedEvent, this, thisTree);
        }
    }
}
