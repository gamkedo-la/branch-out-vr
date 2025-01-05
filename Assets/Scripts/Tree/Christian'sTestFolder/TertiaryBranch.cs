using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TertiaryBranch : TreeLimbBase
{
    TertiaryBranch tertiaryBranchPrefab;
    Leaf leafPrefab;
    float percentageToEndBranch;


    //Initialize if spawned from a branch

    public void Initialize(UnityEvent growEvent, TertiaryBranch previousTertiaryBranch, TreeTest tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTertiaryBranch;
        SetThisTree(tree);
        if (previousTertiaryBranch != null)
        {
            transform.localEulerAngles = GetRandomRotations();
        }
        Initialize();

    } 
    //Initialize if spawned from a trunk
    public void Initialize(UnityEvent growEvent, SecondaryBranch previousSecondaryBranch, TreeTest tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousSecondaryBranch;
        SetThisTree(tree);
        if (previousSecondaryBranch != null)
        {
            transform.localEulerAngles = GetRandomRotations();
        }
        Initialize();

    }
    void Initialize()
    {
        tertiaryBranchPrefab = limbContainer.tertiaryBranch;
        leafPrefab = limbContainer.leaf;
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
    public override void AddChild()
    {
        base.AddChild();
        TreeLimbBase limb = Instantiate(leafPrefab, GetRandomPositionOnLimb(), Quaternion.Euler(GetRandomRotations()), transform);
        branchedLimbs.Add(limb);
        (limb as Leaf).Initialize(GrowthHappenedEvent, this, thisTree);
    }
    public override void Grow()
    {
        base.Grow();

        if (!IsMature)
            return;

        if (LimbTerminated())
            return;

        if (nextLimb == null)
        {
            TreeLimbBase limb = Instantiate(leafPrefab, top.position, Quaternion.Euler(GetRandomRotations()), transform);
            nextLimb = (limb);
            (limb as Leaf).Initialize(GrowthHappenedEvent, this, thisTree);
        }

    }

}
