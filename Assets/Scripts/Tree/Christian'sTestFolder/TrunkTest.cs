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
        transform.localScale = Vector3.one;
        //randomize number of branches on this node
        branchCount = Random.Range(0, 4);


        base.Initialize(growEvent, 1f);
        //this.previousLimb = previousLimb;
/*        if (previousLimb != null)
        {
            transform.localEulerAngles = GetRandomRotations();
        }*/

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void AddChild()
    {
        base.AddChild();
        Debug.Log("Add child to trunk");
        TreeLimbBase limb = Instantiate(branchTestPrefab, GetRandomPositionOnLimb(), Quaternion.Euler(GetRandomRotations()), transform);
        branchedLimbs.Add(limb);
        (limb as BranchTest).Initialize(GrowthHappenedEvent, this);
    }

    //When growth happens trigger the growth event
    public override void Grow()
    {
        base.Grow();

        if (TreeTest.Instance.currentFreeEnergy > 0)
        {
            Energy += 1;
        }
/*        if (!IsMature)
            return;

        //If a next limb in sequence doesn't exist make one
        if (nextLimb == null)
        {
            TreeLimbBase limb = Instantiate(trunkPrefab, top.position, top.rotation, transform);
            nextLimb = (limb);
            (limb as TrunkTest).Initialize(GrowthHappenedEvent, this);
        }*/

    }
}
