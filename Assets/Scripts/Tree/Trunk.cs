using UnityEngine.EventSystems;

public class Trunk : TreePart, IGrowable, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    void IGrowable.Grow()
    {
        throw new System.NotImplementedException();
    }


}
