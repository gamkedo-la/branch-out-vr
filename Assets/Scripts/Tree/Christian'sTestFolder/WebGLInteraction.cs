using UnityEngine;
using UnityEngine.InputSystem;

public class WebGLInteraction : MonoBehaviour
{
    public Vector3 CurrentPosition;
    public InputAction useToolAction,screenPos;
    // Start is called before the first frame update
    void Start()
    {
        useToolAction.Enable();
        useToolAction.performed += _ => Raycast();
        screenPos.Enable();
        screenPos.performed += context => { CurrentPosition = context.ReadValue<Vector2>(); };
    }

    void Raycast()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(CurrentPosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            //suppose i have two objects here named obj1 and obj2.. how do i select obj1 to be transformed 
            if (hit.transform != null && hit.transform.GetComponent<PhysicalInteraction>())
            {
                print(hit.transform.name);
                hit.transform.GetComponent<PhysicalInteraction>().ClickedOn();
            }
        }
    }
}
