using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnergyPathNode : MonoBehaviour
{
    public EnergyPathNode parent;
    public List<EnergyPathNode> children = new();
    public bool root = false;

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
        if (!children.Contains(childNode))
        {
            Debug.LogWarning("The EnergyPathNode is not in the list of children for this Node, but you are trying to remove it.");
        }
        children.Remove(childNode);
        childNode.parent = null;
    }

    /// <summary>
    /// Sort the path waypoints by distance from the parent, to ensure particles move continuously out from the tree rather than haphazardly.
    /// </summary>
    private void SortChildrenByDistance()
    {
        if (children.Count > 1)
        {
            Vector3 parentPosition = transform.position;

            children.Sort((a, b) =>
            {
                if (a != null && b != null)
                {
                    float distA = (a.transform.position - parentPosition).sqrMagnitude;
                    float distB = (b.transform.position - parentPosition).sqrMagnitude;
                    return distA.CompareTo(distB);

                }
                return -1;

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
            if (child != null)
            {
                pathPoints.AddRange(child.GetPathPoints());
            }
        }

        return pathPoints; 
    }
}
