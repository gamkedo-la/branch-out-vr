using Unity.VisualScripting;
using UnityEngine;

public class BranchNode : Branch
{
    [SerializeField]
    GameObject meshRendererObjectForBone;

    public EnergyPathNode pathNode;

    private void Start()
    {
        if (pathNode == null)
        {
            TryGetComponent<EnergyPathNode>(out pathNode);
            if (pathNode == null)
            {
                pathNode = gameObject.AddComponent<EnergyPathNode>(); 
            }
        }
    }

    public override void Trim()
    {
        if (canCut)
        {
            BranchTest proceduralBranch = GetComponentInParent<BranchTest>();
            //TODO: We'll need to either loop through all children or adjust energy tracking in order to remove energy from all child branches when parent is cut
            thisTree.RemoveEnergy(proceduralBranch.Energy);
            meshRendererObjectForBone.SetActive(false);
            canCut = false;
            gameObject.SetActive(false);
            base.Trim();
        }
    }

}
