using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Leaf : TreeLimbBase
{
    float percentageToEndBranch;


    //Initialize if spawned from a branch

    public void Initialize(UnityEvent growEvent, TertiaryBranch previousTertiaryBranch)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTertiaryBranch;
        if (previousTertiaryBranch != null)
        {
            transform.localEulerAngles = GetRandomRotations();
        }
        Initialize();

    } 
    void Initialize()
    {
        //tertiaryBranchPrefab = limbContainer.tertiaryBranch;
        //percentageToEndBranch = Random.value;
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

        if (!IsMature)
            return;
    }

}
