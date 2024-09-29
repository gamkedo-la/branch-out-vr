using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class RaycastOnTrigger : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Button pressed.");
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
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
