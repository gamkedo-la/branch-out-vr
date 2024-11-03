using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class TrunkTest : MonoBehaviour
{
    public UnityEvent GrowthHappenedEvent;
    public Vector2 minRotations,maxRotations;
    public TrunkTest trunkPrefab;
    public TrunkTest previousTrunk;
    public TrunkTest nextTrunk;
    public Transform top;

    //add a random rotation to the trunk and subscribe the growth function to the passed in growth event
    public void Initialize(UnityEvent growEvent, TrunkTest previousTrunk)
    {
        this.previousTrunk = previousTrunk;
        if (previousTrunk != null)
        {
            transform.localEulerAngles = GetRandomRotations();
        }

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

    Vector3 GetRandomRotations()
    {
        Vector3 randomVector = new Vector3();

        randomVector.x = Random.Range(minRotations.x, maxRotations.x);
        randomVector.y = 0f;
        randomVector.z = Random.Range(minRotations.y, maxRotations.y);

        return randomVector;
    }

    //When growth happens trigger the growth event
    public void Grow()
    {
        GrowthHappenedEvent.Invoke();

        if (nextTrunk == null)
        {
            nextTrunk = Instantiate(trunkPrefab, top.position, top.rotation, transform);

            nextTrunk.Initialize(GrowthHappenedEvent, this);
        }

    }
}
