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
        transform.position = defaultPosition.transform.position;
    }

    private void OnDrawGizmos()
    {
                Gizmos.DrawRay(trimRaycastPoint.transform.position, trimRaycastPoint.transform.forward);

    }
    private void RaycastTrim()
    {
        Debug.Log("Raycast trim called.");
        if (!GamePlatformManager.IsVRMode)
        {
            Debug.Log("Raycast in flat mode.");
            if (Physics.Raycast(trimRaycastPoint.transform.position, trimRaycastPoint.transform.forward, out RaycastHit hitInfo, 25, treeLayerForRaycasts))
            {
                GameObject target = hitInfo.collider.gameObject;
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
            Debug.Log("Raycast to check for branch to trim.");
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 25, treeLayerForRaycasts))
            {
                Debug.Log("hit object " + hit.collider.gameObject.name);
                Branch target = hit.collider.gameObject.GetComponent<Branch>();
                if (target)
                {
                    Debug.Log("Trim on branch");
                    target.Trim();
                }
            }
        }
    }

    public override void WebGLSwitchToDifferentTool()
    {
        isActive = false;
        transform.position = defaultPosition.transform.position;
    }

    public override void WebGLMakeActiveTool()
    {
        isActive = true;
    }

    public override void UseTool()
    {
        Debug.Log("Use tool called within scissors script.");
        RaycastTrim();
    }

    public bool CheckIfActive()
    {
        return isActive;
    }
}
