using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchNode : TreePart
{
    [SerializeField] TreeLimbBase thisBranch;

    [SerializeField] GameObject meshRendererObjectForBone;

    [SerializeField] Material deadTreeMaterial;

    public bool isTrunk = false;
    /// <summary>
    /// The EnergyPathNode that's attached to this BranchNode game object.
    /// </summary>
    public EnergyPathNode pathNode;

    private float hideNodeDelay = 0.2f;

    //TODO: make this an array or list to allow for several steps of undo/redo for branch rotations with wire
    private Quaternion previousRotation;
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
        ProceduralTree.OnGameOver += TreeDied;
    }

    /// <summary>
    /// Sets the material to the dead tree material when the tree's energy reaches 0. Going forward, this should be modified to lerp the material over time as the tree is dying. 
    /// </summary>
    private void TreeDied()
    {
        SetMeshRendMat(deadTreeMaterial);
    }

    public void SetMeshRendMat(Material newMat)
    {
        meshRendererObjectForBone.GetComponent<Renderer>().material = newMat;
    }

    public List<BranchNode> GetAffectedBranchesForCut()
    {
        if (!isTrunk)
        {
            List<BranchNode> affectedBranches = new();

            BranchNode[] childNodes = GetComponentsInChildren<BranchNode>();
            for (int i = 0; i < childNodes.Length; i++)
            {
                affectedBranches.Add(childNodes[i]);
            }

            return affectedBranches;
        }
        else
        {
            Debug.Log("Cutting the trunk is not implemented yet.");
            return null;
        }

    }

    public override void Trim()
    {
        if (isTrunk) return;

        thisTree.userCutBranchCount++;

        int nodeIndex = thisBranch.nodes.IndexOf(this);

        if (nodeIndex > 0) 
        {
            // destroy branchedLimbs and nextLimbs from cut node
            List<BranchNode> nodesToDeactivate = new(); // Get a list of the nodes in this branch that will be deactivated by this cut

            foreach (var node in thisBranch.nodes)
            {
                if (thisBranch.nodes.IndexOf(node) >= nodeIndex) // We want to deactivate this node and any of its children
                {
                    nodesToDeactivate.Add(node);
                }
            }

            for (int i = thisBranch.branchedLimbs.Count - 1; i >= 0; i--) 
            {
                var limb = thisBranch.branchedLimbs[i];
                var closestNode = limb.GetClosestNode(limb.transform.position);

                if (nodesToDeactivate.Contains(closestNode)) 
                {
                    thisBranch.branchedLimbs.RemoveAt(i);
                    limb.CutLimb();
                }
            }

            if (thisBranch.nextLimb != null)
            {
                thisBranch.nextLimb.CutLimb();
            }

            if (nodeIndex >= 1)
            {
                // For a partial cut of the branch, remove this objects path node from the previous node
                thisBranch.nodes[nodeIndex - 1].pathNode.RemoveChild(pathNode);
            }
            StartCoroutine(HideandRemoveNodes(nodesToDeactivate));
        }

        else if (nodeIndex == 0)
        {
            if (thisBranch is TertiaryBranch)
            {
                AudioManager.Instance.PlaySFX("SFX_Leaves_Rustle_Short");
            }
            thisBranch.CutLimb();
        }
    }

    private IEnumerator HideandRemoveNodes(List<BranchNode> nodes)
    {
        yield return new WaitForSeconds(hideNodeDelay);

        foreach (var node in nodes)
        {
            if (node != null)
            {
                int index = thisBranch.nodes.IndexOf(node);
                thisBranch.nodes[index].pathNode.RemoveChild(pathNode);
                thisBranch.nodes.Remove(node);
                node.meshRendererObjectForBone.SetActive(false);
                node.gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// Applies rotation to the node based on the players movement when the Wire tool is active and in use.
    /// </summary>
    /// <param name="playerMotionDelta"></param>
    public void ApplyRotation(Vector3 playerMotionDelta)
    {
        float yaw = playerMotionDelta.x;
        float pitch = -playerMotionDelta.y;

        transform.Rotate(Vector3.up, yaw, Space.World);
        transform.Rotate(transform.right, pitch, Space.World);
    }

    public void SetStartingRotation()
    {
        previousRotation = transform.rotation;
    }

    public void RevertRotation()
    {
        Debug.Log("Reverting rotation to " + previousRotation);
        transform.rotation = previousRotation;
    }
    /// <summary>
    /// Make sure to remove this node's path point from the global list when the game object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (pathNode.parent != null)
        {
            pathNode.parent.RemoveChild(pathNode);
        }
        ProceduralTree.OnGameOver -= TreeDied;
    }
}
