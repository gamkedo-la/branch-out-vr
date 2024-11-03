using UnityEngine;

public class Scissors : Tool, IGrabbable
{
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
        toolRB.isKinematic = true;
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

    private void RaycastTrim()
    {
        Debug.Log("Raycast trim called.");
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
