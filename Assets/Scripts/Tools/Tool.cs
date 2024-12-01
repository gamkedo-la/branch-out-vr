using UnityEngine;

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
    }

    public virtual void WebGLSwitchToDifferentTool() 
    {
        isActive = false;
        transform.position = defaultPosition.transform.position;
    }
    public virtual void UseTool() { }

}
