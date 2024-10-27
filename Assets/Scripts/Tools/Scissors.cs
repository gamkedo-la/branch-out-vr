using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Scissors : MonoBehaviour
{
    private InputAction trim;

    [SerializeField]
    LayerMask treeLayer;
    private void OnEnable()
    {
        trim = PlayerInputHandler.Instance.inputActions.FindAction("Trim");

        trim.performed += _ => RaycastTrim();
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
