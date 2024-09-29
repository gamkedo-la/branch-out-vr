using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TreePart : MonoBehaviour
{
    [SerializeField]
    float maxEnergy;

    [SerializeField]
    float currentEnergy;
    public virtual void Trim()
    {
        Destroy(gameObject);
        //cut branch off - for now, just cut entire branch, later on implement separating into 2 GameObject's at the touched point on collider.
    }

    public virtual void Grow()
    {
        
    }
}
