using System.Collections;
using System.Collections.Generic;
using Manicomio.Animation;
using UnityEngine;

namespace Manicomio.ActionableObjects
{
  [RequireComponent(typeof(Animator))]
  public class StandardActionableObject : ActionableObject
  {
    [Header("Animation")]
    [Tooltip("The animation state to play when interacting with this object")]
    [SerializeField] string animationState;

    protected Animator m_animator;

    protected override void InnerStart()
    {
      m_animator = GetComponent<Animator>();
    }

    protected override void InnerInteract()
    {
      UpdateAnimation();
    }

    private void UpdateAnimation()
    {
      m_animator.SetFloat(AnimationConstants.ANIMATION_DIRECTION, !m_interacted ? 1f : -1f);
      m_animator.Play(animationState);
    }

    public override bool IsAnimationPlaying()
    {
      var state = m_animator.GetCurrentAnimatorStateInfo(0);

      Debug.Log(state.fullPathHash);

      return state.IsName(AnimationConstants.ANIMATION_GENERIC_END);
    }
  }
}
