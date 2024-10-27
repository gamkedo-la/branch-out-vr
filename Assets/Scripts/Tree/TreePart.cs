using UnityEngine;

public abstract class TreePart : MonoBehaviour
{
    [SerializeField]
    float maxEnergy;

    [SerializeField]
    float currentEnergy;

    public virtual void Trim()
    {
        //cut branch off - for now, just cut entire branch, later on implement cutting off at closest node.
        Destroy(gameObject);
    }
}
