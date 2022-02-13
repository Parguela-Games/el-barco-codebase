using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPLook : MonoBehaviour {
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    private Transform flashLight;


    [SerializeField]
    [Tooltip("Vertical and horizontal sensitivity")]
    private LookSensitivity lookSensitivity = new LookSensitivity(1f, 0.2f);

    [Serializable]
    public class LookSensitivity {
        [SerializeField]
        protected float vertical;

        public float Vertical {
            get {
                return vertical;
            }
            set {
                vertical = value;
            }
        }

        [SerializeField]
        protected float horizontal;

        public float Horizontal {
            get {
                return horizontal;
            }
            set {
                horizontal = value;
            }
        }

        public LookSensitivity(float horizontal, float vertical) {
            this.horizontal = horizontal;
            this.vertical = vertical;
        }
    }

    private Rigidbody m_rigidbody;
    private CharacterController m_characterController;

    private GameActions gameActions;

    // Pitch is the "yes" movement
    private float m_cameraPitch = 0f;
    // Yaw is the "no" movement"
    private float m_cameraYaw = 0f;

    private void Awake() {
        gameActions = new GameActions();
        gameActions.Player.ShowCursor.performed += ShowCursor;
        PlayerEvents.OnPlayerActivated += () => enabled = true;
        PlayerEvents.OnPlayerDeactivated += () => enabled = false;

        // Hide the cursor until the player wants to show it (only dev mode)
        Cursor.visible = false;
    }

    private void OnEnable() {
        gameActions.Player.Enable();
    }

    private void OnDisable() {
        gameActions.Player.ShowCursor.performed -= ShowCursor;
        PlayerEvents.OnPlayerActivated -= () => enabled = true;
        PlayerEvents.OnPlayerDeactivated -= () => enabled = false;

        gameActions.Player.Disable();
    }

    void Start() {
        if (!TryGetComponent<Rigidbody>(out m_rigidbody) && !TryGetComponent<CharacterController>(out m_characterController)) {
            throw new Exception("No rigidbody nor characterController defined");
        }
    }

    void Update() {
        ProcessLook();
    }

    private void ProcessLook() {
        var lookDelta = gameActions.Player.Look.ReadValue<Vector2>();

        // Apply the vertical camera movement and limit it between 90 and -90 degrees
        m_cameraPitch = Mathf.Clamp(m_cameraPitch + -lookDelta.y * lookSensitivity.Vertical, -90, 90);
        // Do the same thing to the horizontal movement but don't limit it cause the player can turn around
        m_cameraYaw = m_cameraYaw + lookDelta.x * lookSensitivity.Horizontal;

        Quaternion rotation = Quaternion.Euler(0f, m_cameraYaw, 0f);

        if (m_rigidbody != null) {
            // The rigidbody only rotates along the Y axis (horizontal) so it can not end facing down, but we can still apply
            // a relative force
            m_rigidbody.MoveRotation(rotation);
        } else {
            m_characterController.transform.eulerAngles = rotation.eulerAngles;
        }

        // Store the cure rotation to preserve the Z position
        Vector3 currRotation = virtualCamera.transform.eulerAngles;

        // The camera, instead, rotates along the X and Y axes so the player can look above and sideways
        virtualCamera.transform.eulerAngles = new Vector3(m_cameraPitch, m_cameraYaw, currRotation.z);
        flashLight.eulerAngles = new Vector3(m_cameraPitch, m_cameraYaw, currRotation.z);
    }

    private void ShowCursor(InputAction.CallbackContext obj) {
        Cursor.visible = true;
    }
}
