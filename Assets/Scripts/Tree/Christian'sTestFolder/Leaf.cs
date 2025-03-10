using UnityEngine;
using UnityEngine.Events;

public class Leaf : TreeLimbBase
{
    [SerializeField] ParticleSystem leafParticles;

    private bool donePlaying = false;
    private float particlesStopTime;

    private void OnEnable()
    {
        EnergyTickTimer.Tick += Photosynthesis;
    }

    //Initialize if spawned from a branch
    public void Initialize(UnityEvent growEvent, TertiaryBranch previousTertiaryBranch, ProceduralTree tree)
    {
        base.Initialize(growEvent);
        this.previousLimb = previousTertiaryBranch;
        SetThisTree(tree);
        if (previousTertiaryBranch != null)
        {
            transform.localEulerAngles = GetRandomBranchRotation();
        }
        particlesStopTime = leafParticles.main.duration - 1;
    } 

    private void StopLeafParticlePlay()
    {
        leafParticles.Pause();
    }

    /// <summary>
    /// Every leaf node will create energy for the tree every Tick.
    /// </summary>
    private void Photosynthesis()
    {
        if (thisTree != null)
        {
            thisTree.UpdateEnergy(energySystemValues.leafEnergyGainPerTick); 
        }
    }

    public override void Grow()
    {
        if (donePlaying)
        {
            return;
        }
        else if (leafParticles.time >= particlesStopTime && leafParticles.isPlaying)
        {
            StopLeafParticlePlay();
            donePlaying = true;
        }
    }

    private void OnDisable()
    {
        EnergyTickTimer.Tick -= Photosynthesis;
    }

    private void OnDestroy()
    {
        EnergyTickTimer.Tick -= Photosynthesis;
    }
}
