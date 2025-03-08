using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Limb Table", menuName = "ScriptableObjects/Limb Table", order = 1)]
public class LimbContainer : ScriptableObject
{
    public Trunk trunkTest;
    public Branch branchTest;
    public SecondaryBranch secondaryBranch;
    public TertiaryBranch tertiaryBranch;
    public Leaf leaf;
}
