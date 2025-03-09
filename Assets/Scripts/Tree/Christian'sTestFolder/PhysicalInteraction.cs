using UnityEngine;
using UnityEngine.Events;

public class PhysicalInteraction : MonoBehaviour
{
    public UnityEvent ClickedOnEvent;
    public void ClickedOn()
    {

        ClickedOnEvent.Invoke();
    }
}
