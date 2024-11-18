using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchSection : Branch
{
    [SerializeField]
    GameObject meshRendererObjectForBone;

    public override void Trim()
    {
        if (canCut)
        {
            meshRendererObjectForBone.SetActive(false);
            canCut = false;
            base.Trim();
        }
    }

}
