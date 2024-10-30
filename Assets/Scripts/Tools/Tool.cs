using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Tool : MonoBehaviour
{
    
    public LayerMask treeLayerForRaycasts;

    
    public GameObject defaultPosition;

    public bool isActiveTool;

    public GameObject toolAttachPoint;

    public InputActionReference useTool;

    public Rigidbody toolRB;

    public virtual void MakeActiveTool()
    {
        isActiveTool = true;
    }

    public virtual void DropTool()
    {
        isActiveTool = false;
    }
}
