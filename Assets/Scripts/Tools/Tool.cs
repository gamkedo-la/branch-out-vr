using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    
    public LayerMask branchNodeLayerForTools;

    public GameObject defaultTransform;

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
        transform.position = defaultTransform.transform.position;
        transform.rotation = defaultTransform.transform.rotation;
    }
    public virtual void UseTool() { }

}
