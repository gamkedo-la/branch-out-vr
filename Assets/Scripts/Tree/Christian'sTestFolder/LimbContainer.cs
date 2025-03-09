using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Limb Table", menuName = "ScriptableObjects/Limb Table", order = 1)]
public class LimbContainer : ScriptableObject
{
    public Trunk trunk;
    public Branch taperedPrimaryBranch;
    public Branch nonTaperedPrimaryBranch;
    public SecondaryBranch taperedSecondaryBranch;
    public SecondaryBranch nonTaperedSecondaryBranch;
    public TertiaryBranch taperedTertiaryBranch;
    public TertiaryBranch nonTaperedTertiaryBranch;
    public Leaf leaf;
}
