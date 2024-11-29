using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PhysicalInteraction : MonoBehaviour
{
    public UnityEvent ClickedOnEvent;
    public void ClickedOn()
    {

        ClickedOnEvent.Invoke();
    }
}
