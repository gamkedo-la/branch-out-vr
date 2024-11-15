using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SecondaryBranch : TreeLimbBase
{
    TertiaryBranch tertiaryBranchPrefab;
    SecondaryBranch secondaryBranchPrefab;
    float percentageToEndBranch;


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
    //Initialize if spawned from a trunk
    public void Initialize(UnityEvent growEvent, SecondaryBranch previousTrunk)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTrunk;
        if (previousTrunk != null)
        {
            transform.localEulerAngles = GetRandomRotations();
        }
        Initialize();

    }
    void Initialize()
    {
        tertiaryBranchPrefab = limbContainer.tertiaryBranch;
        secondaryBranchPrefab = limbContainer.secondaryBranch;
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

    void HandleBranches()
    {
        TreeLimbBase limb = Instantiate(tertiaryBranchPrefab, GetRandomPositionOnLimb(), Quaternion.Euler(GetRandomRotations()), transform);
        branchedLimbs.Add(limb);
        (limb as TertiaryBranch).Initialize(GrowthHappenedEvent, this);
    }

    public override void Grow()
    {
        base.Grow();

        if (!IsMature)
            return;

        if (WillGrowSub())
            HandleBranches();

        if (nextLimb == null && WillLimbContinue())
        {
            TreeLimbBase limb = Instantiate(secondaryBranchPrefab, top.position, top.rotation, transform);
            nextLimb = (limb);
            (limb as SecondaryBranch).Initialize(GrowthHappenedEvent, this);
        }

    }

}
