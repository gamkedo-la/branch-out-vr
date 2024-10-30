using UnityEngine;

public class Scissors : Tool, IGrabbable
{
    private void OnEnable()
    {
        useTool.action.performed += _ => RaycastTrim();
    }

    void IGrabbable.OnGrab()
    {
        if (!GamePlatformManager.IsVRMode)
        {
            return;
        }
        MakeActiveTool();
        toolRB.isKinematic = true;
        
    }

    void IGrabbable.OnRelease()
    {
        if (!GamePlatformManager.IsVRMode)
        {
            return;
        }
        toolRB.isKinematic = false;
    }

    private void RaycastTrim()
    {
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

    public override void MakeActiveTool()
    {
        base.MakeActiveTool();
    }

    public override void DropTool()
    {
        base.DropTool();
    }

    private void OnDisable()
    {
        useTool.action.performed -= _ => RaycastTrim();
    }
}
