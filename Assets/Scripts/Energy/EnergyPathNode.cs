using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnergyPathNode : MonoBehaviour
{
    public EnergyPathNode parent;
    public List<EnergyPathNode> children = new();
    public bool root = false;

/*    private void Awake()
    {
        if (parent == null && !root)
        {
            if (transform.parent != null)
            {
                transform.parent.TryGetComponent<EnergyPathNode>(out parent);
                if (parent != null)
                {
                    parent.AddChild(this);
                }
            }
        }
    }
*/
    public void AddChild(EnergyPathNode childNode)
    {
        children.Add(childNode);
        if (childNode.parent != null )
        {
            childNode.parent = GetComponent<EnergyPathNode>();
        }
    }

    public void RemoveChild(EnergyPathNode childNode)
    {
        children.Remove(childNode);
        childNode.parent = null;
    }

    /// <summary>
    /// Sort the path waypoints by distance from the parent, to ensure particles move continuously out from the tree rather than haphazardly.
    /// </summary>
    private void SortChildrenByDistance()
    {
        if (children.Count > 0)
        {
            Vector3 parentPosition = transform.position;

            children.Sort((a, b) =>
            {
                float distA = (a.transform.position - parentPosition).sqrMagnitude;
                float distB = (b.transform.position - parentPosition).sqrMagnitude;

                return distA.CompareTo(distB);
            });
        }

    }
    /// <summary>
    /// Recursively gather all EnergyPathNodes in the hierarchy.
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetPathPoints()
    {
        SortChildrenByDistance();
        //Start the list with this object's transform
        List<Transform> pathPoints = new() { transform };

        foreach (EnergyPathNode child in children)
        {
            pathPoints.AddRange(child.GetPathPoints());
        }

        return pathPoints; 
    }
}
