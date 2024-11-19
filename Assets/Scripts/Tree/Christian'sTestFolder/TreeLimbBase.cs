using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TreeLimbBase : MonoBehaviour
{
    public bool IsMature {  
        get {return isMature; }
        private set
        {
            isMature = value;
        } 
    }
    [SerializeField]
    bool isMature;
    public float MaturityPercent
    {
        get { return maturityPercent; }
        private set
        {
            maturityPercent = Mathf.Clamp01(value);
            SetSize(maturityPercent);
            IsMature = maturityPercent >= 1;
        }
    }
    [SerializeField]
    float maturityPercent;

    public int maxChildLimbCount = 2;
    float growSubChance = .25f;
    float terminateChance = .5f;
    public LimbContainer limbContainer;

    public Vector2 minRotations, maxRotations;

    public TreeLimbBase previousLimb;
    public List<TreeLimbBase> branchedLimbs;
    public TreeLimbBase nextLimb;

    public Transform top;


    public UnityEvent GrowthHappenedEvent = new UnityEvent();
    public virtual void Initialize(UnityEvent growEvent)
    {
        MaturityPercent = 0f;
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

    public virtual void AddChild()
    {

    }
    public void SetSize(float maturityPercent)
    {
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, maturityPercent);
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
        
        if(MaturityPercent < 1)
        {
            MaturityPercent += .1f; 
            return;
        }

        if(branchedLimbs.Count < maxChildLimbCount && WillGrowSub())
            AddChild();

    }
    public bool WillGrowSub()
    {
        return Random.value < growSubChance;
    }
    public bool WillLimbContinue()
    {
        return Random.value < terminateChance;
    }
}
