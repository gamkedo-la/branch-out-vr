using UnityEngine;
using UnityEngine.Events;

public class Branch : TreeLimbBase
{
    private SecondaryBranch secondaryBranchPrefab;
    private Branch taperedBranchPrefab; // This is used at the end of a branch that has multiple limbs
    private Branch nonTaperedBranchPrefab;

    [SerializeField] private float terminateChance = 0.65f;
    protected override float TerminateChance => terminateChance;

    //Initialize if spawned from a trunk
    public void Initialize(UnityEvent growEvent, Trunk previousTrunk, ProceduralTree tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTrunk;
        SetThisTree(tree);
        thisTree.growingLimbs.Add(this);
        if (previousTrunk != null)
        {
            transform.localEulerAngles = GetRandomBranchRotation(); // Get the rotation this branch grows out of the previous branch
        }
        thisTree.UpdateGlobalPath(); // Update energy path points with our new branch nodes
        Initialize();

    }

    //Initialize if spawned from a branch
    public void Initialize(UnityEvent growEvent, Branch previousBranch, ProceduralTree tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousBranch;
        SetThisTree(tree);
        thisTree.growingLimbs.Add(this);
        if (previousBranch != null)
        {
            transform.localEulerAngles = GetRandomBranchRotation(); // Get the rotation this branch grows out of the previous branch
        }

        CheckLimbTerminated();
        thisTree.UpdateGlobalPath(); // Update energy path points with our new branch nodes
        Initialize();
    }
    void Initialize()
    {
        taperedBranchPrefab = limbContainer.taperedPrimaryBranch;
        nonTaperedBranchPrefab = limbContainer.nonTaperedPrimaryBranch;
        secondaryBranchPrefab = limbContainer.taperedSecondaryBranch;
        
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

        BranchNode parentNode = GetClosestNodeToBranch(nextChildGrowPosition);

        TreeLimbBase limb = 
            Instantiate(
                secondaryBranchPrefab, 
                nextChildGrowPosition, 
                Quaternion.Euler(nextChildGrowRotation), 
                parentNode.transform);
        branchedLimbs.Add(limb);
        EnergyPathNode energyPath = parentNode.gameObject.GetComponent<EnergyPathNode>();
        energyPath.AddChild(limb.nodes[0].GetComponent<EnergyPathNode>());
        limb.nodes[0].pathNode.parent = energyPath;
        (limb as SecondaryBranch).Initialize(GrowthHappenedEvent, this, thisTree);

        nextChildGrowPosition = GetRandomPositionOnLimb();
        nextChildGrowRotation = GetRandomBranchRotation();
    }

    public override void Grow()
    {
        base.Grow();

        if (!IsMature || nextLimb != null) 
            return;
        
        // Determine if this is the final branch segment
        if (!IsLimbTerminated)
        {
            CheckLimbTerminated();
        }

        if (nextLimb == null)
        {
            // if this is the last limb, use the tapered branch. Otherwise, use the non-tapered branch.
            TreeLimbBase prefabToUse = 
                IsLimbTerminated ? 
                taperedBranchPrefab : 
                nonTaperedBranchPrefab;

            TreeLimbBase limb = Instantiate(prefabToUse, top.position, top.rotation, nodes[^1].transform);
            nextLimb = limb;
            EnergyPathNode energyPath = nodes[^1].gameObject.GetComponent<EnergyPathNode>();
            energyPath.AddChild(limb.nodes[0].GetComponent<EnergyPathNode>());
            (limb as Branch).Initialize(GrowthHappenedEvent, this, thisTree);
        }
    }
}
