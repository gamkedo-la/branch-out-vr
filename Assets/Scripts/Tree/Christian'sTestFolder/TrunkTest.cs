using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class TrunkTest : TreeLimbBase
{
    public int branchCount;

    public BranchTest branchTestPrefab;
    public TrunkTest trunkPrefab;

    public void Initialize(UnityEvent growEvent, TrunkTest previousLimb, TreeTest tree)
    {
        //find the trunk reference from the lookup table
        //needed for reusing prefab
        trunkPrefab = limbContainer.trunkTest;
        branchTestPrefab = limbContainer.branchTest;
        transform.localScale = Vector3.one;
        //randomize number of branches on this node
        branchCount = Random.Range(0, 4);
        thisTree = transform.parent.GetComponent<TreeTest>();
        pathNode = GetComponent<EnergyPathNode>();
        thisTree.UpdateGlobalPath();
        nextChildGrowPosition = GetRandomPositionOnLimb();
        nextChildGrowRotation = GetRandomBranchRotation();
        base.Initialize(growEvent, 1f);
        thisTree.growingLimbs.Add(this);
    }

    public override void AddChild()
    {
        base.AddChild();
        TreeLimbBase limb = Instantiate(branchTestPrefab, nextChildGrowPosition, Quaternion.Euler(nextChildGrowRotation), transform);
        branchedLimbs.Add(limb);
        (limb as BranchTest).Initialize(GrowthHappenedEvent, this, thisTree);
        if (pathNode != null)
        {
            pathNode.AddChild(limb.nodes[0].pathNode);
        }
        nextChildGrowPosition = GetRandomPositionOnLimb();
        nextChildGrowRotation = GetRandomBranchRotation();
    }

    //When growth happens trigger the growth event
    public override void Grow()
    {
        base.Grow();
        
        if (thisTree.currentFreeEnergy > 0)
        {
            Energy += 1;
            thisTree.UpdateEnergy(-1);
            thisTree.ReleaseAllocatedEnergy(1);
        }


    }
}
