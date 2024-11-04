using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TreeLimbBase : MonoBehaviour
{
    public LimbContainer limbContainer;

    public Vector2 minRotations, maxRotations;

    public TreeLimbBase previousLimb;
    public List<TreeLimbBase> branchedLimbs;
    public TreeLimbBase nextLimb;

    public Transform top;


    public UnityEvent GrowthHappenedEvent;
    public virtual void Initialize(UnityEvent growEvent)
    {
        growEvent.AddListener(Grow);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 GetRandomRotations()
    {
        Vector3 randomVector = new Vector3();

        randomVector.x = Random.Range(minRotations.x, maxRotations.x);
        randomVector.y = 0f;
        randomVector.z = Random.Range(minRotations.y, maxRotations.y);

        return randomVector;
    }
    public Vector3 GetRandomPositionOnLimb()
    {
        return Vector3.Lerp(transform.position, top.position, Random.value);
    }
    public virtual void Grow()
    {
        GrowthHappenedEvent.Invoke();


    }
}
