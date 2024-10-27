using UnityEngine;
using UnityEngine.EventSystems;

public class Branch : TreePart, IGrowable, IPointerClickHandler
{
    [SerializeField]
    bool isMain;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Trim branch");
        Trim();
    }

    public void Grow()
    {
        throw new System.NotImplementedException();
    }
}
