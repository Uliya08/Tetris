using UnityEngine;

public class MenuAnimationController : MonoBehaviour
{
    public Animator animator;

    public void PlayAnimation(string triggerName)
    {
        animator.SetTrigger(triggerName);   
    }
}
