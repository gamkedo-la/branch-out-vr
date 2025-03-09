using UnityEngine;

[CreateAssetMenu(fileName = "New Energy Values Asset", menuName = "Energy Variable Values")]
public class EnergySystemValues : ScriptableObject
{
    //THIS IS NOT COMPLETE but gives an idea of some of the goals for the energy system - complexity will be cut if needed.

    //For clarity:
    //In the visualization system, the energy particles will move throughout the tree on a path. 
    //"Pooling" energy means energy particles in the visualization system that leave the flow path and bunch or clump around a spot.
    //We will need to keep track of the total FREE energy of a tree system. 
    //Pooled energy is removed from the total energy of a tree, as it will be "consumed" for growth each tick until it is all gone.
    //If the pooled energy reaches the minimum requirement for growth, that energy cannot ever be returned to the system.
    //If it has not reached the minimum requirement for growth, and the total free energy gets too low, it will be returned to the system. 

    [Tooltip("Energy points that a leaf group brings in every tick.")]
    public float leafEnergyGainPerTick = 1.0f;

    [Tooltip("The maximum number of limbs that can grow at any one time. If this limit is reached, energy will not pool for new limb growth.")]
    public int maxLimbsGrowing = 5;

    [Tooltip("If the tree's total energy falls below this value, it is dying (but can still recover).")]
    public int treeDyingThresholdAmount = 100;

    [Tooltip("If the tree's total energy falls below this value, it is dead and cannot recover.")]
    public int treeDeadAmount = 50;

    [Tooltip("If the tree looses this percent of its total energy in one action, it is dying (but can still recover).")]
    public float treeDyingThresholdPercentLoss = 50.0f;

    [Tooltip("If the tree looses this percent of its total energy in one action, it is dead and cannot recover.")]
    public float treeDeadPercent = 90.0f;

    [Tooltip("When the tree becomes stressed due to frequent watering or cutting off parts of the tree, multiply energy gain by this value to reduce the rate.")]
    public float stressedEnergyGainModifier = 0.25f;

    [Tooltip("To prevent a tree pooling all or most of its energy, set a minimum total amount - if below this, energy will not pool at growth locations. See comments at top of script for how to handle already pooled energy.")]
    public float minTotalEnergyForGrowth = 5000.0f;

    //Before the new branch grows, the energy visual system will show the energy particles gathering at the location where the branch will grow. 
    //Once the minimum value is reached, there is a chance the branch will grow every Tick; this chance increases the more pooled energy there is. 
    [Tooltip("Minimum pooled energy requirement that must be reached before a new main branch can grow from the trunk at pre-designated location.")]
    public float minEnergyForMainBranch = 100.0f;

    [Tooltip("Minimum pooled energy requirement that must be reached before a new secondary branch can grow from a main branch at pre-designated location.")]
    public float minEnergyForSecondaryBranch = 75.0f;

    [Tooltip("Minimum pooled energy requirement that must be reached before a new tertiary branch can grow from a secondary branch at pre-designated location.")]
    public float minEnergyForTertiaryBranch = 50.0f;

    [Tooltip("The rate energy is consumed per tick to grow a new branch (influences branch size, or scale)")]
    public float energyConsumptionRateForGrowth = 1;

    
}
