using UnityEngine;
using UnityEngine.Events;

public class SecondaryBranch : TreeLimbBase
{
    private TertiaryBranch taperedTertiaryBranchPrefab;
    private SecondaryBranch taperedSecondaryBranchPrefab;
    //private SecondaryBranch nonTaperedSecondaryBranchPrefab;

    [SerializeField] private float terminateChance = 0.7f;
    protected override float TerminateChance => terminateChance;

    //Initialize if spawned from a branch
    public void Initialize(
        UnityEvent growEvent, Branch previousBranch, ProceduralTree tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousBranch;
        SetThisTree(tree);
        thisTree.growingLimbs.Add(this);
        if (previousBranch != null)
        {
            transform.localEulerAngles = GetRandomBranchRotation();
        }

        thisTree.UpdateGlobalPath();
        Initialize();

    } 
    //Initialize if spawned from a secondary branch
    public void Initialize(UnityEvent growEvent, SecondaryBranch previousTrunk, ProceduralTree tree, bool isLastLimb)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTrunk;
        SetThisTree(tree);
        thisTree.growingLimbs.Add(this);
        if (previousTrunk != null)
        {
            transform.localEulerAngles = GetRandomBranchRotation();
        }

        IsLimbTerminated = isLastLimb;
        thisTree.UpdateGlobalPath();
        Initialize();
    }
    void Initialize()
    {
        taperedTertiaryBranchPrefab = limbContainer.taperedTertiaryBranch;
        taperedSecondaryBranchPrefab = limbContainer.taperedSecondaryBranch;
        //nonTaperedSecondaryBranchPrefab = limbContainer.nonTaperedSecondaryBranch;
    }

    public override void AddChild()
    {
        base.AddChild();

        BranchNode parentNode = GetClosestNode(nextChildGrowPosition);
        Debug.Log("parentNode for the new child: " + parentNode.name);
        Debug.Log($"next child grow position for for new tertiary branch from node {parentNode.name}: " + nextChildGrowPosition);
        Debug.Log($"next child grow rotation for new tertiary branch from node {parentNode.name}: " + nextChildGrowRotation);

        if (nextChildGrowPosition == Vector3.zero)
        {
            Debug.LogError("next grow position is 0");
        }
        if (nextChildGrowRotation == Vector3.zero)
        {
            Debug.LogError("next grow position is 0");
        }

        Debug.DrawLine(transform.position, top.position, Color.magenta, 5f);


        TreeLimbBase limb = 
            Instantiate(
                taperedTertiaryBranchPrefab, 
                nextChildGrowPosition, 
                Quaternion.Euler(nextChildGrowRotation), 
                parentNode.transform);

        branchedLimbs.Add(limb);
        EnergyPathNode energyPath = parentNode.gameObject.GetComponent<EnergyPathNode>();
        energyPath.AddChild(limb.nodes[0].GetComponent<EnergyPathNode>());
        limb.nodes[0].pathNode.parent = energyPath;
        (limb as TertiaryBranch).Initialize(GrowthHappenedEvent, this, thisTree);
        //when switched to BranchNode growing child, add logic for Bone0 EnergyPathNode to have this node as parent for calculating path

        nextChildGrowPosition = GetRandomPositionOnLimb();
        nextChildGrowRotation = GetRandomBranchRotation();
    }
    

    public override void Grow()
    {
        base.Grow();

        if (!IsMature || nextLimb != null) 
            return;

        if (IsLimbTerminated) return;

        // Determine if this is the final branch segment
        IsLimbTerminated = CheckLimbTerminated();

        if (nextLimb == null)
        {
            // Last limb? Use tapered secondary branch. No? Use the non-tapered secondary branch.
            /*            TreeLimbBase prefabToUse = 
                            IsLimbTerminated ? 
                            taperedSecondaryBranchPrefab : 
                            nonTaperedSecondaryBranchPrefab;*/

            // remove non-tapered branches for now
            TreeLimbBase prefabToUse = taperedSecondaryBranchPrefab;
            TreeLimbBase limb = Instantiate(prefabToUse, top.position, top.rotation, transform);
            nextLimb = limb;

            EnergyPathNode energyPath = nodes[^1].gameObject.GetComponent<EnergyPathNode>();
            energyPath.AddChild(limb.nodes[0].GetComponent<EnergyPathNode>());

            (limb as SecondaryBranch).Initialize(GrowthHappenedEvent, this, thisTree, IsLimbTerminated);
        }
    }
}
