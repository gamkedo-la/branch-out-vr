using System.Collections.Generic;
using UnityEngine;

public class Scissors : Tool, IGrabbable
{
    private TreePart lastNearestBranch;

    private void Start()
    {
        isActive = false;
    }

    void IGrabbable.OnGrab(GameObject leftOrRightHand)
    {
        if (!GamePlatformManager.IsVRMode)
        {
            return;
        }
        isActive = true;
    }

    void IGrabbable.OnRelease()
    {
        if (!GamePlatformManager.IsVRMode)
        {
            return;
        }
        isActive = false;
        transform.SetPositionAndRotation(defaultTransform.transform.position, defaultTransform.transform.rotation);
    }

    private void Update()
    {
        if (isActive)
        {
            TreePart closestBranch = ClosestBranch();

            if (lastNearestBranch != closestBranch)
            {

                if (lastNearestBranch != null)
                {
                    List<BranchNode> lastAffectedNodes = lastNearestBranch.GetComponent<BranchNode>().GetAffectedBranchesForCut();
                    if (lastAffectedNodes.Count > 0)
                    {
                        foreach (BranchNode node in lastAffectedNodes)
                        {
                            node.SetMeshRendMat(branchDefaultMat);
                        }
                    }
                }
                if (closestBranch != null)
                {
                    List<BranchNode> currentAffectedNodes = closestBranch.GetComponent<BranchNode>().GetAffectedBranchesForCut();
                    if (currentAffectedNodes.Count > 0)
                    {
                        foreach (BranchNode node in currentAffectedNodes)
                        {
                            node.SetMeshRendMat(targetMat);
                        }
                    }
                }
                lastNearestBranch = closestBranch;
            }
        }
        else
        {
            if (lastNearestBranch != null)
            {
                List<BranchNode> lastAffectedNodes = lastNearestBranch.GetComponent<BranchNode>().GetAffectedBranchesForCut();
                if (lastAffectedNodes.Count > 0)
                {
                    foreach (BranchNode node in lastAffectedNodes)
                    {
                        node.SetMeshRendMat(branchDefaultMat);
                    }
                }
                lastNearestBranch = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        //visualize the area for cutting branches
        Gizmos.DrawSphere(proximityPoint.transform.position, overlapSphereRadius);
    }

    
    void TrimBranch()
    {
        TreePart closestBranch = ClosestBranch();
        if (closestBranch != null)
        {
            AudioManager.Instance.PlaySFX("SFX_ScissorCut");
            closestBranch.Trim();
        }
        else
        {
            Debug.Log("No branches close enough to trim.");
        }
    }

    public override void WebGLSwitchToDifferentTool()
    {
        base.WebGLSwitchToDifferentTool();
    }

    public override void WebGLMakeActiveTool(GameObject currentPlayerObject)
    {
        AudioManager.Instance.PlaySFX("SFX_ScissorEquip");
        base.WebGLMakeActiveTool();
    }

    public override void UseTool()
    {
        if (isActive)
        {
            TrimBranch();
        }
    }

    public bool CheckIfActive()
    {
        return isActive;
    }
}
