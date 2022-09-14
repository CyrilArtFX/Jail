using System.Collections;
using UnityEngine;

namespace Jail.Interactables.Triggers
{
    public class AnimatorTrigger : BaseTrigger
    {
        [Header("Animator"), SerializeField]
        Animator animator;
        [SerializeField]
        string animationName;

        protected override void OnTrigger(Collider other, bool is_enter)
        {
            if (is_enter)
            {
                animator.Play(animationName);
            }
        }
    }
}