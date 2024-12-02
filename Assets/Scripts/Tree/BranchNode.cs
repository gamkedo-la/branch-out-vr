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
            meshRendererObjectForBone.SetActive(false);
            canCut = false;
            gameObject.SetActive(false);
            base.Trim();
        }
    }

}
