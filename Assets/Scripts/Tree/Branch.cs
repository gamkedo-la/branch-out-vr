using UnityEngine;
using UnityEngine.EventSystems;

public class Branch : TreePart, IGrowable, IPointerClickHandler
{
    [SerializeField]
    bool isMain;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Trim branch");
        rb.isKinematic = false;
        Trim();
    }

    public void Grow()
    {
        throw new System.NotImplementedException();
    }
}
