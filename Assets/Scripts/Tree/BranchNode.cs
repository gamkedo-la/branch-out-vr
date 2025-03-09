using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
        Debug.Log("Trim from BranchNode");
        int nodeIndex = thisBranch.nodes.IndexOf(this);

        if (nodeIndex >= 0) {
            // destroy branchedLimbs and nextLimbs from cut node
            while (thisBranch.branchedLimbs.Count > 0) {
                var limb = thisBranch.branchedLimbs[thisBranch.branchedLimbs.Count - 1];
                limb.CutLimb();
            }
            thisBranch.branchedLimbs.Clear();
            if (thisBranch.nextLimb) {
                thisBranch.nextLimb.CutLimb();
                Destroy(thisBranch.nextLimb);
            }

            // destroy branchedLimbs and nextLimbs from cut node's children
            BranchNode cutNodeBrandNodeComponents = GetComponent<BranchNode>();
            BranchNode[] cutNodeChildren = 
                cutNodeBrandNodeComponents.GetComponentsInChildren<BranchNode>();
            foreach (var child in cutNodeChildren) {
                while (child.thisBranch.branchedLimbs.Count > 0) {
                    var limb = child.thisBranch.branchedLimbs[thisBranch.branchedLimbs.Count - 1];
                    limb.CutLimb();
                }
                child.thisBranch.branchedLimbs.Clear();
                if (child.thisBranch.nextLimb) {
                    child.thisBranch.nextLimb.CutLimb();
                    Destroy(child.thisBranch.nextLimb);
                }
            }

            // deactivate the rendering and gameObject of the cut node 
            // and the following nodes on the cut branch
            foreach (var node in thisBranch.nodes) {
                if (thisBranch.nodes.IndexOf(node) <= nodeIndex) {
                    continue;
                }
                node.meshRendererObjectForBone.SetActive(false);
                node.gameObject.SetActive(false);
            }
        }

        thisBranch.nodes.RemoveRange(nodeIndex, thisBranch.nodes.Count - nodeIndex - 1);

        //deactivate this object and all child nodes; this allows player to cut branches in sections
        meshRendererObjectForBone.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ApplyRotation(Vector3 playerMotionDelta)
    {
        Debug.Log(playerMotionDelta);
        Vector3 currentRotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        Quaternion newRotation = new Quaternion(currentRotation.x + playerMotionDelta.x, currentRotation.y + playerMotionDelta.y, currentRotation.z + playerMotionDelta.z, Quaternion.identity.w);

        transform.rotation = newRotation;
    }
}
