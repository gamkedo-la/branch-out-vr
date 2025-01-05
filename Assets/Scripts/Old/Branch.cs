using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Branch : TreePart, IGrowable, IPointerClickHandler
{
    [SerializeField]
    private bool isMain;

    [SerializeField]
    List<BranchNode> nodes;

    [SerializeField]
    GameObject secondaryBranchPrefab;

    [SerializeField]
    bool test = false;

    private void Start()
    {
        canCut = true;
        if (test == true)
        {
            test = false;
            GameObject temp = Instantiate(secondaryBranchPrefab);
            temp.transform.SetParent(nodes[2].transform, true);
            temp.transform.localPosition = Vector3.zero;
        }

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
