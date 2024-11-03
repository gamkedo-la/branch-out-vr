using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Tool : MonoBehaviour
{
    
    public LayerMask treeLayerForRaycasts;

    public GameObject defaultPosition;

    public GameObject toolAttachPoint;

    public bool isActive = false;

    public Rigidbody toolRB;

    public virtual void WebGLMakeActiveTool() { }

    public virtual void WebGLSwitchToDifferentTool() { }
    public virtual void UseTool() { }

}
