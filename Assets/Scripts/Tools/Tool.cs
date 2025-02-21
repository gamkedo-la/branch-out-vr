using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    
    public LayerMask branchNodeLayerForTools;

    public GameObject defaultTransform;

    public GameObject toolAttachPoint;

    public bool isActive = false;

    public GameObject proximityPoint;


    /// <summary>
    /// Size of the sphere that checks for branches to cut in VR mode. 
    /// </summary>
    public float overlapSphereRadius = 0.35f;

    public Material branchDefaultMat;
    public Material targetMat;

    public virtual void WebGLMakeActiveTool(GameObject currentPlayerObject = null) 
    {
        isActive = true;
    }

    public virtual void WebGLSwitchToDifferentTool() 
    {
        isActive = false;
        transform.SetPositionAndRotation(defaultTransform.transform.position, defaultTransform.transform.rotation);
    }

    /// <summary>
    /// Determine the closest BranchNode that the tool can interact with. 
    /// </summary>
    public TreePart ClosestBranch()
    {
        TreePart closestBranch = null;
        //are any branches close enough to cut? 
        Collider[] closestBranches = Physics.OverlapSphere(proximityPoint.transform.position, overlapSphereRadius, branchNodeLayerForTools);

        if (closestBranches.Length > 0)
        {
            float closestDistance = overlapSphereRadius;
            //loop through any branches close enough to find the closest one
            for (int i = 0; i < closestBranches.Length; i++)
            {
                if (!closestBranches[i].TryGetComponent<TreePart>(out var hasTpScript))
                {
                    continue;
                }
                Vector3 closestPointOnCollider = closestBranches[i].ClosestPoint(proximityPoint.transform.position);
                float distance = Vector3.Distance(proximityPoint.transform.position, closestPointOnCollider);
                if (distance < closestDistance)
                {
                    closestBranch = hasTpScript;
                    closestDistance = distance;
                }
            }
        }

        return closestBranch;
    }

    public virtual void UseTool() { }

}
