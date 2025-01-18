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

    private void OnDrawGizmos()
    {
        //visualize the line for cutting branches
        Gizmos.DrawRay(trimRaycastPoint.transform.position, trimRaycastPoint.transform.forward * 10);
    }
    /// <summary>
    /// Cuts the tree at a branch determined by a raycast from the scissors tip in WebGL, or by proximity in VR. - TODO: move functionality for finding nearby branches so that we can
    /// highlight the branch that will be cut for player before they use the tool
    /// </summary>
    private void TrimTree()
    {
        //if we're in WebGL mode, use a raycast to determine branch to cut
        if (!GamePlatformManager.IsVRMode)
        {
            //does this line up correctly in play mode? 
            if (Physics.Raycast(trimRaycastPoint.transform.position, transform.forward, out RaycastHit hit, 100f, branchNodeLayerForTools))
            {
                GameObject target = hit.collider.gameObject;
                if (target != null)
                {
                    Debug.Log(target.name);
                    if (target.TryGetComponent<TreePart>(out TreePart part))
                    {
                        AudioManager.Instance.PlaySFX("SFX_ScissorCut");
                        part.Trim();
                    }
                }
            }
        }
        //if in VR mode, use proximity to determine branch to cut
        if (GamePlatformManager.IsVRMode)
        {
            //are any branches close enough to cut? 
            Collider[] closestBranches = Physics.OverlapSphere(transform.position, overlapSphereRadius, branchNodeLayerForTools);

            if (closestBranches.Length > 0)
            {
                GameObject closestBranch = null;
                float closestDistance = overlapSphereRadius;
                //loop through any branches close enough to find the closest one
                for (int i = 0; i < closestBranches.Length; i++)
                {
                    Vector3 closestPointOnCollider = closestBranches[i].ClosestPoint(trimRaycastPoint.transform.position);
                    float distance = Vector3.Distance(transform.position, closestPointOnCollider);
                    if (distance < closestDistance)
                    {
                        closestBranch = closestBranches[i].gameObject;
                        closestDistance = distance;
                    }
                }

                if (closestBranch != null)
                {
                    closestBranch.TryGetComponent<TreePart>(out TreePart part);
                    part.Trim();
                }
            }

            else
            {
                Debug.Log("No branches close enough to trim.");
            }

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
