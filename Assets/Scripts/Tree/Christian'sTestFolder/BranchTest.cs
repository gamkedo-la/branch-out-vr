using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BranchTest : TreeLimbBase
{
    BranchTest branchPrefab;
    float percentageToEndBranch;

    //Initialize if spawned from a trunk
    public void Initialize(UnityEvent growEvent, TrunkTest previousTrunk)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTrunk;
        if (previousTrunk != null)
        {
            transform.localEulerAngles = GetRandomRotations();
        }
        Initialize();

    }
    //Initialize if spawned from a branch

    public void Initialize(UnityEvent growEvent, BranchTest previousBranch)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousBranch;
        if (previousBranch != null)
        {
            transform.localEulerAngles = GetRandomRotations();
        }
        Initialize();

    }
    void Initialize()
    {
        branchPrefab = limbContainer.branchTest;
        percentageToEndBranch = Random.value;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Grow()
    {
        base.Grow();
        if (nextLimb == null)
        {
            TreeLimbBase limb = Instantiate(branchPrefab, top.position, top.rotation, transform);
            nextLimb = (limb);
            (limb as BranchTest).Initialize(GrowthHappenedEvent, this);
        }

    }
}
