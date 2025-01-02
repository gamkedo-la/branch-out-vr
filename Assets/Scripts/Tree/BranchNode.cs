using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchNode : Branch
{
    [SerializeField]
    GameObject meshRendererObjectForBone;

    public override void Trim()
    {
        if (canCut)
        {
            BranchTest proceduralBranch = GetComponentInParent<BranchTest>();
            //TODO: We'll need to either loop through all children or adjust energy tracking in order to remove energy from all child branches when parent is cut
            TreeTest.Instance.RemoveEnergy(proceduralBranch.Energy);
            meshRendererObjectForBone.SetActive(false);
            canCut = false;
            gameObject.SetActive(false);
            base.Trim();
        }
    }

}
