using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TreeTest : MonoBehaviour
{
    public TrunkTest trunkTest;
    public float growthTime = 2f;
    [SerializeField]
    float progress = 0;


    public UnityEvent GrowthHappenedEvent;
    // Start is called before the first frame update
    void Start()
    {
        //create the first node of the trunk and initialize it
        TrunkTest temp = Instantiate(trunkTest, transform, false);

        temp.Initialize(GrowthHappenedEvent, null);

    }

    // Update is called once per frame
    void Update()
    {
        //timer for when the growth will happen
        progress += Time.deltaTime;

        if (progress > growthTime)
            Grow();
    }

    //Reset timer and raise event for all nodes in the tree to update
    public void Grow()
    {
        progress = 0;

        GrowthHappenedEvent?.Invoke();
    }
}
