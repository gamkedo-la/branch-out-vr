using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BranchNode : TreePart
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

    public override void Trim()
    {
        //TODO: Don't remove thisBranch completely; remove all nextLimb (recursive) and branchedLimbs
        Debug.Log("Trim from BranchNode");
        int nodeIndex = thisBranch.nodes.IndexOf(this);

        if (thisBranch.nodes.Count >= nodeIndex)
        {
            if (thisBranch.previousLimb.nodes.Count > 1)
            {
                EnergyPathNode energyPath = thisBranch.previousLimb.nodes[nodeIndex].GetComponent<EnergyPathNode>();
                energyPath.RemoveChild(energyPath.GetComponentsInChildren<EnergyPathNode>()[1]);
            }
        }

        thisBranch.CutLimb();

        thisBranch.nodes.RemoveRange(nodeIndex, thisBranch.nodes.Count - nodeIndex - 1);

        //deactivate this object and all child nodes; this allows player to cut branches in sections
        meshRendererObjectForBone.SetActive(false);
        gameObject.SetActive(false);
        //base.Trim();
    }

    public void ApplyRotation(Vector3 playerMotionDelta)
    {
        Debug.Log(playerMotionDelta);
        Vector3 currentRotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        Quaternion newRotation = new Quaternion(currentRotation.x + playerMotionDelta.x, currentRotation.y + playerMotionDelta.y, currentRotation.z + playerMotionDelta.z, Quaternion.identity.w);

        transform.rotation = newRotation;
    }
}
