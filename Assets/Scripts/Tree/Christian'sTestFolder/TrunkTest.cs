using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class TrunkTest : TreeLimbBase
{
    int branchesCreated;
    public int branchCount;

    public BranchTest branchTestPrefab;
    public TrunkTest trunkPrefab;


    //add a random rotation to the trunk and subscribe the growth function to the passed in growth event
    public void Initialize(UnityEvent growEvent, TrunkTest previousLimb)
    {
        //find the trunk reference from the lookup table
        //needed for reusing prefab
        trunkPrefab = limbContainer.trunkTest;
        branchTestPrefab = limbContainer.branchTest;

        //randomize number of branches on this node
        branchCount = Random.Range(0, 4);


        base.Initialize(growEvent);
        this.previousLimb = previousLimb;
        if (previousLimb != null)
        {
            transform.localEulerAngles = GetRandomRotations();
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void HandleBranches()
    {
        TreeLimbBase limb = Instantiate(branchTestPrefab, GetRandomPositionOnLimb(), Quaternion.Euler(GetRandomRotations()), transform);
        branchedLimbs.Add(limb);
        (limb as BranchTest).Initialize(GrowthHappenedEvent, this);
    }


    //When growth happens trigger the growth event
    public override void Grow()
    {
        base.Grow();

        if (!IsMature)
            return;

        //right now creates branches based on a random amount should switch to a percentage chance
        if (WillGrowSub())
            HandleBranches();

        //If a next limb in sequence doesn't exist make one
        if (nextLimb == null)
        {
            TreeLimbBase limb = Instantiate(trunkPrefab, top.position, top.rotation, transform);
            nextLimb = (limb);
            (limb as TrunkTest).Initialize(GrowthHappenedEvent, this);
        }

    }
}
