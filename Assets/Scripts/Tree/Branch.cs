using UnityEngine;
using UnityEngine.EventSystems;

public class Branch : TreePart, IPointerClickHandler
{
    [SerializeField]
    bool isMain;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Trim branch");
        Trim();
    }
}
