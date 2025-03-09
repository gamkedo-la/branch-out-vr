using UnityEngine;

// Deprecated and should likely be refactored out
public abstract class TreePart : MonoBehaviour
{
    [SerializeField]
    float maxEnergy;

    [SerializeField]
    float currentEnergy;

    [SerializeField]
    Rigidbody rb;

    public ProceduralTree thisTree;

    public bool canCut = true;

    public virtual void Trim()
    {
        
        Debug.Log("Trim from TreePart");
    }
}
