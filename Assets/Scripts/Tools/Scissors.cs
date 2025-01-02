using UnityEngine;

public class Scissors : Tool, IGrabbable
{
    [SerializeField]
    private GameObject trimRaycastPoint;
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
                Gizmos.DrawRay(trimRaycastPoint.transform.position, trimRaycastPoint.transform.forward * 10);

    }

    private void Update()
    {
        if (isActive)
        {
            trimRaycastPoint.transform.forward = Camera.main.transform.forward;
        }
    }
    /// <summary>
    /// Cuts the tree at a branch determined by a raycast from the scissors tip in WebGL, or by proximity in VR.
    /// </summary>
    private void TrimTree()
    {
        
        if (!GamePlatformManager.IsVRMode)
        {
            if (Physics.Raycast(trimRaycastPoint.transform.position, transform.forward, out RaycastHit hit, 100f, branchNodeLayerForTools))
            {
                GameObject target = hit.collider.gameObject;
                if (target != null)
                {
                    Debug.Log(target.name);
                    if (target.TryGetComponent<TreePart>(out TreePart part))
                    {
                        part.Trim();
                    }
                }
            }
        }

        if (GamePlatformManager.IsVRMode)
        {
            Collider[] closestBranches = Physics.OverlapSphere(transform.position, 5f, branchNodeLayerForTools);

            if (closestBranches.Length > 0)
            {
                GameObject closestBranch = null;
                float closestDistance = 5f;
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
        base.WebGLMakeActiveTool();
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
