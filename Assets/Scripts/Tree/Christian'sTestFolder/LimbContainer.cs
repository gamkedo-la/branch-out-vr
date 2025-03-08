using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Limb Table", menuName = "ScriptableObjects/Limb Table", order = 1)]
public class LimbContainer : ScriptableObject
{
    public Trunk trunk;
    public Branch branch;
    public SecondaryBranch secondaryBranch;
    public TertiaryBranch tertiaryBranch;
    public Leaf leaf;
}
