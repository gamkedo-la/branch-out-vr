using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BranchNode : Branch
{
    [SerializeField] TreeLimbBase thisBranch;

    [SerializeField] GameObject meshRendererObjectForBone;

    public EnergyPathNode pathNode;

    private void Start()
    {
        name += " " + Random.Range(0, 1000);
        if (pathNode == null)
        {
            TryGetComponent<EnergyPathNode>(out pathNode);
            if (pathNode == null)
            {
                pathNode = gameObject.AddComponent<EnergyPathNode>(); 
            }
        }
    }

    public void SetMeshRendMat(Material newMat)
    {
        meshRendererObjectForBone.GetComponent<Renderer>().material = newMat;
    }

    public List<BranchNode> GetAffectedBranchesForCut()
    {
        List<BranchNode> affectedBranches = new();

        BranchNode[] childNodes = GetComponentsInChildren<BranchNode>();
        for (int i = 0; i < childNodes.Length; i++)
        {
            affectedBranches.Add(childNodes[i]);
        }

        if (thisBranch.nextLimb != null)
        {
            //TODO: add all next limbs 
        }

        if (thisBranch.branchedLimbs.Count > 0)
        {
            //TODO: add all branched limbs and their next limbs
        }

        return affectedBranches;
    }

    public void PreviewAffectedBranchesForWire()
    {

    }

    public override void Trim()
    {
        //TODO: Don't remove thisBranch completely; remove all nextLimb (recursive) and branchedLimbs
/*            int nodeIndex = thisBranch.nodes.IndexOf(this);

        if (nodeIndex != -1)
        {
            thisBranch.nodes.RemoveRange(nodeIndex, thisBranch.nodes.Count - nodeIndex - 1);
        }*/

        thisBranch.CutLimb();
            
        //deactivate this object and all child nodes; this allows player to cut branches in sections
        meshRendererObjectForBone.SetActive(false);
        gameObject.SetActive(false);
        base.Trim();
    }
}
