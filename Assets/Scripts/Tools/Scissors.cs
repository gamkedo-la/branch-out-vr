using UnityEngine;
using UnityEngine.InputSystem;

public class Scissors : MonoBehaviour, IGrabbable
{
    private InputAction trim;

    [SerializeField]
    LayerMask treeLayer;
    private void OnEnable()
    {
        trim = PlayerInputManager.Instance.inputActions.FindAction("Trim");

        trim.performed += _ => RaycastTrim();
    }

    void IGrabbable.OnGrab()
    {
        if (!GamePlatformManager.IsVRMode)
        {
            return;
        }
        
        throw new System.NotImplementedException();
    }

    void IGrabbable.OnRelease()
    {
        if (!GamePlatformManager.IsVRMode)
        {
            return;
        }

        throw new System.NotImplementedException();
    }

    private void RaycastTrim()
    {
        if (GamePlatformManager.IsVRMode)
        {
            Debug.Log("Raycast to check for branch to trim.");
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 25, treeLayer))
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
}
