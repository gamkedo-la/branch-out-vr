using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TreeLimbBase : MonoBehaviour
{
    public EnergySystemValues energySystemValues;
    #region Energy
    public float Energy 
    {
        get { return energy; } 
        set {energy = value;}
    }
    [SerializeField]
    float energy = 0;

    public UnityEvent EnergyDepletedEvent;
    #endregion

    public TreeTest thisTree;
    public bool cut = false;
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
    public bool terminated;
    float terminateChance = .5f;
    public LimbContainer limbContainer;

    public Vector2 minRotations, maxRotations, minNodeRotation, maxNodeRotation;

    public TreeLimbBase previousLimb;
    public List<TreeLimbBase> branchedLimbs;
    public TreeLimbBase nextLimb;

    public Transform top;
    public List<BranchNode> nodes;

    public Rigidbody rigidbody;

    public UnityEvent GrowthHappenedEvent = new UnityEvent();

    public EnergyPathNode pathNode;

    public Vector3 nextChildGrowPosition, nextChildGrowRotation;
    public virtual void Initialize(UnityEvent growEvent, float maturity = 0f)
    {
        MaturityPercent = maturity;
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
        Energy -= 50;
    }

    public virtual void SetThisTree(TreeTest tree)
    {
        thisTree = tree;
        if (nodes.Count > 0)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].thisTree = tree;
            }
        }

    }

    public float TakeEnergy(float amount)
    {
        if (Energy > 0)
        {
            Energy -= amount;
            return amount;
        }
        else
        {
            return 0;
        }
    }
    public void SetSize(float maturityPercent)
    {
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, maturityPercent);
    }

    public Vector3 GetRandomBranchRotation()
    {
        Vector3 randomVector = new()
        {
            x = Random.Range(minRotations.x, maxRotations.x),
            y = 0f,
            z = Random.Range(minRotations.y, maxRotations.y)
        };

        return randomVector;
    }

    public Vector3 GetRandomPositionOnLimb()
    {
        return Vector3.Lerp(transform.position, top.position, Random.value);
    }
    public virtual void Grow()
    {
        if(previousLimb != null)
        {
            Energy += previousLimb.TakeEnergy(1);
        }

        if (Energy <= 0)
            return;

        GrowthHappenedEvent.Invoke();
        
        if(MaturityPercent < 1)
        {
            MaturityPercent += .01f; 
            return;
        }

        if(branchedLimbs.Count < maxChildLimbCount && WillGrowSub())
            AddChild();

    }
    public bool WillGrowSub()
    {
        return energy>=100;
    }
    public bool LimbTerminated()
    {
        if(terminated == false)
            terminated = Random.value < terminateChance;
        
        return terminated;
    }
    public virtual void CutLimb()
    {
        cut = true;

        transform.parent = null;

        TurnOnPhysics();

        if(previousLimb.nextLimb == this)
        previousLimb.nextLimb = null;

        if (previousLimb.branchedLimbs.Contains(this))
            previousLimb.branchedLimbs.Remove(this);

        previousLimb.GrowthHappenedEvent.RemoveListener(Grow);

        StartCoroutine(ShrinkLimbForDeletion());
    }

    public virtual void TurnOnPhysics()
    {

        rigidbody.isKinematic = false;

/*        foreach(TreeLimbBase treeLimbBase in branchedLimbs)
        {
            TurnOnPhysics();
        }*/

/*        if(nextLimb)
            nextLimb.TurnOnPhysics();*/
    }

    public void OnDestroy()
    {
/*        foreach(TreeLimbBase treeLimbBase in branchedLimbs)
        {
            Destroy(treeLimbBase.gameObject);
        }*/
    }

    public IEnumerator ShrinkLimbForDeletion()
    {
        float progress = 0f;
        while(progress < 1)
        {
            progress += .00001f;

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, progress);

            yield return new WaitForSeconds(.01f);
        }

        Destroy(this.gameObject);
    }
}
