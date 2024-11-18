using UnityEngine;
using UnityEngine.EventSystems;

public class Branch : TreePart, IGrowable, IPointerClickHandler
{
    [SerializeField]
    private bool isMain;


    private void Start()
    {
        canCut = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
/*        Debug.Log("Trim branch");
        Trim();*/
    }

    public void Grow()
    {
        throw new System.NotImplementedException();
    }
}
