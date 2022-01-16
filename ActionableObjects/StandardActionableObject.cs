using System.Collections;
using System.Collections.Generic;
using Manicomio.Animation;
using UnityEngine;

namespace Manicomio.ActionableObjects {
    [RequireComponent(typeof(Animator))]
    public class StandardActionableObject : ActionableObject {
        [Header("Animation")]
        [Tooltip("The animation state to play when interacting with this object")]
        [SerializeField] string animationState;

        protected Animator m_animator;

        protected override void InnerStart() {
            m_animator = GetComponent<Animator>();
        }

        protected override void InnerInteract() {
            UpdateAnimation();
        }

        private void UpdateAnimation() {
            IsAnimationPlaying();
            if (m_interacted) {
                m_animator.SetTrigger("close");
            } else {
                m_animator.SetTrigger("open");
            }
        }

        public override bool IsAnimationPlaying() {
            var state = m_animator.GetCurrentAnimatorStateInfo(0);

            AnimatorClipInfo[] clips = m_animator.GetCurrentAnimatorClipInfo(0);

            if (clips.Length > 0) Debug.Log(clips[0].clip.name);

            return state.IsName(AnimationConstants.ANIMATION_GENERIC_END);
        }
    }
}
