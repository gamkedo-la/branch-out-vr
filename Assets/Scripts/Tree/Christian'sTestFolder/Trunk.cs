using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Trunk : TreeLimbBase
{
    public int branchCount;

    private Branch taperedBranchPrefab;

    private Trunk trunkPrefab;
    private Branch nonTaperedBranchPrefab;


    public void Initialize(UnityEvent growEvent)
    {
        //find the trunk reference from the lookup table
        //needed for reusing prefab

        trunkPrefab = limbContainer.trunk;
        taperedBranchPrefab = limbContainer.taperedPrimaryBranch;
        nonTaperedBranchPrefab = limbContainer.nonTaperedPrimaryBranch;
        transform.localScale = Vector3.one;
        //randomize number of branches on this node
        branchCount = Random.Range(0, 6);
        thisTree = transform.parent.GetComponent<ProceduralTree>();
        nextChildGrowPosition = GetRandomPositionOnLimb();
        nextChildGrowRotation = GetRandomBranchRotation();
        base.Initialize(growEvent, 1f);
        thisTree.growingLimbs.Add(this);
    }

    public override void AddChild()
    {
        base.AddChild();
        TreeLimbBase limb = Instantiate(nonTaperedBranchPrefab, nextChildGrowPosition, Quaternion.Euler(nextChildGrowRotation), transform);
        branchedLimbs.Add(limb);
        EnergyPathNode lastTrunkNode = nodes[^1].GetComponent<EnergyPathNode>();
        lastTrunkNode.AddChild(limb.nodes[0].pathNode);
        limb.nodes[0].pathNode.parent = lastTrunkNode;
        (limb as Branch).Initialize(GrowthHappenedEvent, this, thisTree);
        nextChildGrowPosition = GetRandomPositionOnLimb();
        nextChildGrowRotation = GetRandomBranchRotation();
    }

    //When growth happens trigger the growth event
    public override void Grow()
    {
        base.Grow();
        
        if (thisTree.currentFreeEnergy > 0)
        {
            //thisTree.ReleaseAllocatedEnergy(1);
            Energy += 1;
            thisTree.UpdateEnergy(-1 + Time.deltaTime);
        }
    }
}
