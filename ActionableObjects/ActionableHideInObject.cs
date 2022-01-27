using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manicomio.ActionableObjects {
    public class ActionableHideInObject : ActionableObject {
        [Header("Animation")]
        [Tooltip("The animation state to play when interacting with this object")]
        [SerializeField] string animationState;

        [SerializeField]
        private CinemachineVirtualCamera hiddenCamera;

        [SerializeField] InteractionText interactText;

        Camera m_mainCamera;

        protected Animator m_animator;

        private bool m_animPlaying = false;

        private Renderer m_objectRenderer;

        private GameActions m_gameActions;

        private void Awake() {
            m_gameActions = new GameActions();
            m_gameActions.Player.Disable();
            m_gameActions.Player.Interact.performed += OnInteractPerformed;
        }

        private void OnDestroy() {
            m_gameActions.Player.Interact.performed -= OnInteractPerformed;
        }

        protected override void InnerStart() {
            m_animator = GetComponent<Animator>();
            m_objectRenderer = GetComponentInChildren<Renderer>();
            m_mainCamera = Camera.main;
        }

        private void OnInteractPerformed(InputAction.CallbackContext obj) {
            base.Interact();
        }

        protected override void InnerInteract(GameObject interactor = null) {
            m_animator.Play(animationState, 0, 0f);

            if (!m_interacted) {
                // Put the player inside the wardrobe
                PlayerEvents.NotifyPlayerDeactivation();

                hiddenCamera.enabled = true;

                interactText.SetInteractionText(this.GetInteractionTextReverse());
                interactText.Enable();
                m_gameActions.Player.Enable();
            } else {
                hiddenCamera.enabled = false;

                PlayerEvents.NotifyPlayerActivation();

                m_gameActions.Player.Disable();
            }
        }

        public override bool IsAnimationPlaying() => m_animPlaying;

        public void AnimationStarted() {
            m_animPlaying = true;
        }

        public void AnimationFinished() {
            m_animPlaying = false;
        }
    }
}