using UnityEngine;

public class Scissors : Tool, IGrabbable
{
    [SerializeField]
    private GameObject trimRaycastPoint;

    /// <summary>
    /// Size of the sphere that checks for branches to cut in VR mode. 
    /// </summary>
    [SerializeField]
    private float overlapSphereRadius = 5f;

    public Material branchDefaultMat;
    public Material targetMat;

    TreePart lastNearestBranch = null;

    private void Start()
    {
        isActive = false;
    }

    void IGrabbable.OnGrab()
    {
        if (!GamePlatformManager.IsVRMode)
        {
            return;
        }
        isActive = true;
        //toolRB.isKinematic = true;
    }

    void IGrabbable.OnRelease()
    {
        if (!GamePlatformManager.IsVRMode)
        {
            return;
        }
        //toolRB.isKinematic = false;
        isActive = false;
        transform.position = defaultTransform.transform.position;
    }

    void Update()
    {
        TreePart closestBranch = ClosestBranch();

        if (lastNearestBranch != closestBranch)
        {
            if(lastNearestBranch != null)
            {
                lastNearestBranch.GetComponent<BranchNode>().SetMeshRendMat(branchDefaultMat);
            }
            if (closestBranch != null)
            {
                closestBranch.GetComponent<BranchNode>().SetMeshRendMat(targetMat);
            }
            lastNearestBranch = closestBranch;
        }
    }

    private void OnDrawGizmos()
    {
        //visualize the line for cutting branches
        Gizmos.DrawSphere(trimRaycastPoint.transform.position, overlapSphereRadius);
    }
    /// <summary>
    /// Cuts the tree at a branch determined by a raycast from the scissors tip in WebGL, or by proximity in VR. - TODO: move functionality for finding nearby branches so that we can
    /// highlight the branch that will be cut for player before they use the tool
    /// </summary>
    private TreePart ClosestBranch()
    {
        TreePart closestBranch = null;
        //are any branches close enough to cut? 
        Collider[] closestBranches = Physics.OverlapSphere(trimRaycastPoint.transform.position, overlapSphereRadius, branchNodeLayerForTools);

        if (closestBranches.Length > 0)
        {
            float closestDistance = overlapSphereRadius;
            //loop through any branches close enough to find the closest one
            for (int i = 0; i < closestBranches.Length; i++)
            {
                TreePart hasTpScript = closestBranches[i].GetComponent<TreePart>();
                if(hasTpScript == null)
                {
                    continue;
                }
                Vector3 closestPointOnCollider = closestBranches[i].ClosestPoint(trimRaycastPoint.transform.position);
                float distance = Vector3.Distance(trimRaycastPoint.transform.position, closestPointOnCollider);
                if (distance < closestDistance)
                {
                    closestBranch = hasTpScript;
                    closestDistance = distance;
                }
            }
        }

        return closestBranch;
    }
    void TrimTree()
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

    public override void WebGLMakeActiveTool()
    {
        AudioManager.Instance.PlaySFX("SFX_ScissorEquip");
        base.WebGLMakeActiveTool();
        //trimRaycastPoint.transform.forward = Camera.main.transform.forward;
    }

    public override void UseTool()
    {
        TrimTree();
    }

    public bool CheckIfActive()
    {
        return isActive;
    }
}
