using UnityEngine;
using UnityEngine.InputSystem;

public class WebGLPlayerController : MonoBehaviour
{
    [SerializeField]
    Camera webGLCamera;

    [SerializeField]
    Tool activeTool;

    [SerializeField]
    InputActionAsset inputActions;

    private InputAction mousePosition;

    private Vector3 playerPosition;

    private void OnEnable()
    {
        mousePosition = inputActions.FindAction("Position");
        mousePosition.performed += inputContext => UpdatePlayerPosition(inputContext);
    }

    private void Start()
    {
        playerPosition = Vector3.zero;
    }

    private void UpdatePlayerPosition(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input != null)
        {
            Debug.Log(input);
            Vector3 mouseViewportPos = webGLCamera.ScreenToViewportPoint(input);
            mouseViewportPos.z = 1.6f;
            playerPosition = webGLCamera.ViewportToWorldPoint(mouseViewportPos);
            transform.position = playerPosition;
        }
    }

    private void OnDisable()
    {
        mousePosition.performed -= inputContext => UpdatePlayerPosition(inputContext);

    }
}
