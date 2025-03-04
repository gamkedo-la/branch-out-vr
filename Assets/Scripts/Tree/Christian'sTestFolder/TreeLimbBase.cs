using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TreeLimbBase : MonoBehaviour
{
    #region Energy
    [Header("Energy")]
    public EnergySystemValues energySystemValues;

    [SerializeField]
    float energy = 0;
    public float Energy 
    { 
        get { return energy; } 
        set { energy = value; }
    }

    public float allocatedEnergy = 0f;

    public bool allocateEnergyForGrowth = false;


    public UnityEvent EnergyDepletedEvent;

    public EnergyPathNode pathNode;

    #endregion

    [Header("Growth")]
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
    public bool terminated;
    float terminateChance = 0.2f;
    public LimbContainer limbContainer;

    public Vector2 minRotations, maxRotations;

    public Vector3 nextChildGrowPosition, nextChildGrowRotation;


    public TreeLimbBase previousLimb;
    public List<TreeLimbBase> branchedLimbs;
    public TreeLimbBase nextLimb;

    public Transform top;
    public List<BranchNode> nodes;


    public UnityEvent GrowthHappenedEvent = new UnityEvent();

    [Header("Other")]
    public Rigidbody _rigidbody;



    public virtual void Initialize(UnityEvent growEvent, float maturity = 0f)
    {
        name += " " + Random.Range(0, 1000);
        MaturityPercent = maturity;
        growEvent.AddListener(Grow);
    }

    public void AllocateEnergy(float amount)
    {
        allocatedEnergy += amount;
        if (allocatedEnergy > Energy)
        {
            Debug.LogWarning("Limb is attempting to allocate more energy than is available.");
        }
        thisTree.AllocateEnergy(amount);
    }
    public virtual void AddChild()
    {
        Energy -= 50;
        allocateEnergyForGrowth = false;
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

    /// <summary>
    /// This should use the available allocated/pooled energy from the previous limb for growing new limbs.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public float TakeEnergy(float amount)
    {
        if (Energy > 0)
        {
            Energy -= amount;

            //CHRISTIAN NOTE : removing this so that tree limbs will take energy from there parent dynamically

            //allocatedEnergy -= amount;

            //thisTree.UpdateEnergy(-amount);
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

    public BranchNode GetClosestNodeToBranch()
    {
        if (nodes.Count > 0)
        {
            float closestDistance = 100f;
            BranchNode closestNode = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                float distance = Vector3.Distance(nextChildGrowPosition, nodes[i].transform.position);

                if (distance <= closestDistance)
                {
                    closestDistance = distance;
                    closestNode = nodes[i];
                }
            }

            return closestNode;
        }
        else
        {
            return null;
        }
    }
    public virtual void Grow()
    {
        if (previousLimb != null)
        {
            if (previousLimb.Energy > 0)
            {
                Energy += previousLimb.TakeEnergy(1);
            }
        }

        if (Energy <= 0) return;

        GrowthHappenedEvent?.Invoke();

        if (MaturityPercent < 1)
        {
            MaturityPercent += .01f;
            return;
        }

        if (branchedLimbs.Count < maxChildLimbCount && WillGrowSub())
            AddChild();
    }
    public bool WillGrowSub()
    {
        return energy >= 50;
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


        if (previousLimb.nextLimb == this)
        previousLimb.nextLimb = null;

        if (previousLimb.branchedLimbs.Contains(this))
            previousLimb.branchedLimbs.Remove(this);

        previousLimb.GrowthHappenedEvent.RemoveListener(Grow);

        StartCoroutine(ShrinkLimbForDeletion());
    }

    public virtual void TurnOnPhysics()
    {
        if (_rigidbody != null && _rigidbody.isKinematic)
        {
            _rigidbody.isKinematic = false;
        }

        if (nextLimb)
            nextLimb.TurnOnPhysics();
    }

    public void OnDestroy()
    {
        if (nextLimb)
        {
            Destroy(nextLimb);
        }
    }

    public IEnumerator ShrinkLimbForDeletion()
    {
        float progress = 0f;
        while (progress <= 1)
        {
            progress += .00001f;

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, progress);

            yield return new WaitForSeconds(.01f);
        }
        Destroy(gameObject);
    }
}
