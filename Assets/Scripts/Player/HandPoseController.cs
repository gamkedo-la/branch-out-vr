using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Handles controlling the pose and animations of a player's hand. 
/// </summary>
public class HandPoseController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    public void Grab()
    {
        //logic for grab animation
    }

    public void HoldTool(string toolName)
    {
        toolName = toolName.FirstCharacterToUpper();
        //string animatorParameter = $"Has{toolName}";
        string animatorParameter = "HasScissors";
        animator.SetBool(animatorParameter, true);
    }

    public void NoTool(string lastToolName)
    {
        lastToolName = lastToolName.FirstCharacterToUpper();
        //string animatorParameter = $"Has{lastToolName}";
        string animatorParameter = "HasScissors";
        animator.SetBool(animatorParameter, false);
    }
}
