using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Tool : MonoBehaviour
{
    
    public LayerMask treeLayerForRaycasts;

    public GameObject defaultPosition;

    public GameObject toolAttachPoint;

    public bool isActive = false;

    public Rigidbody toolRB;

    public virtual void WebGLMakeActiveTool() 
    {
        isActive = true;
        Debug.Log("make active");
    }

    public virtual void WebGLSwitchToDifferentTool() 
    {
        Debug.Log("switch tool");
        isActive = false;
        transform.position = defaultPosition.transform.position;
    }
    public virtual void UseTool() { }

}
