using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class FPControllerCharacter : MonoBehaviour {
    [Header("Movement")]
    [Tooltip("Max movement speed")]
    [SerializeField] float movementSpeed = 5f;
    [Tooltip("How fast the character will reach the max speed")]
    [SerializeField] float acceleration = 5f;
    [Tooltip("How fast the character will reach the 0 speed")]
    [SerializeField] float deceleration = 10f;


    [Tooltip("Factor that will divide the max speed when walking")]
    [SerializeField] float walkDivider = 2f;
    private Vector3 m_playerVelocity;
    private Vector3 m_moveVector;
    private Vector3 m_lastMoveVector;


    [Header("Ground detection")]
    [SerializeField] LayerMask groundMask;
    private bool groundCheck = true;
    private float groundOffset = 0.1f;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 20f;
    private bool m_jumping;

    private GameActions gameActions;

    // Components
    private CharacterController m_characterController;
    private SprintController m_sprintController;
    private CrouchController m_crouchController;
    private Vector3 m_gravity = Vector3.zero;
    private float m_currSpeed = 0f;

    // States
    private bool m_walking = false;

    private void Awake() {
        gameActions = new GameActions();

        // Just binds callbacks with game actions
        SetupCallbacks();
    }

    private void OnEnable() {
        gameActions.Player.Enable();
    }

    private void OnDisable() {
        gameActions.Player.Disable();
    }

    private void OnDestroy() {
        // Just unbinds callbacks from game actions
        TeardownCallbacks();
    }

    void Start() {
        m_characterController = GetComponent<CharacterController>();
        m_sprintController = GetComponent<SprintController>();
        m_crouchController = GetComponent<CrouchController>();
    }

    private void FixedUpdate() {
        GroundCheck();
    }

    // Update is called once per frame
    void Update() {
        ProcessInput();
        ProcessMovement();
    }

    private void ProcessInput() {
        Vector2 readAction = gameActions.Player.Move.ReadValue<Vector2>();

        // Multiply each vector direction with the value read from the input
        Vector3 forwardVector = transform.forward * readAction.y;
        Vector3 rightVector = transform.right * readAction.x;

        // Combines them two and normalizes it to avoid faster diagonal movement
        m_moveVector = (forwardVector + rightVector).normalized;
    }

    /// <summary>
    /// Processes player input to compute the player movement
    /// </summary>
    private void ProcessMovement() {
        // Ensure that the y velocity does not tend to infinite 
        if (groundCheck && m_playerVelocity.y < Mathf.Epsilon) {
            m_playerVelocity.y = -2f;
        }

        /*
        * Importance order:
        *   - Crouching
        *   - Walking
        *   - Sprinting
        */
        // Reduce the speed if the player is crouching
        if (m_crouchController?.GetCrouchDivider() != 1) {
            m_moveVector /= m_crouchController.GetCrouchDivider();
        } else if (m_walking) {
            // Reduce the speed if the player is walking
            m_moveVector /= walkDivider;
        } else if (m_sprintController) {
            // Increase the speed if the player is sprinting
            m_moveVector *= m_sprintController.GetSprintMultiplier();
        }

        if (m_moveVector.magnitude > Mathf.Epsilon && m_currSpeed < movementSpeed) {
            m_currSpeed = Mathf.Clamp(m_currSpeed + acceleration * Time.deltaTime, 0f, movementSpeed);
        } else if (m_moveVector.magnitude <= Mathf.Epsilon && m_currSpeed > Mathf.Epsilon) {
            m_currSpeed = Mathf.Clamp(m_currSpeed - deceleration * Time.deltaTime, 0f, movementSpeed);
            m_moveVector = m_lastMoveVector;
        } else if (m_moveVector.magnitude <= Mathf.Epsilon && m_currSpeed <= Mathf.Epsilon) {
            m_currSpeed = 0f;
        }

        // Apply max speed to the move vector and scale based on delta time to adjust speed to framerate
        m_characterController.Move(m_currSpeed * m_moveVector * Time.deltaTime);

        // Apply gravity to player velocity
        m_playerVelocity += Physics.gravity * Time.deltaTime;

        // Apply velocity to movement (should be only vertical)
        m_characterController.Move(m_playerVelocity * Time.deltaTime);
        m_lastMoveVector = m_moveVector;
    }

    /// <summary>Casts a sphere from player position (center), with same radius as the player colider, 
    /// perpendicular to a horizontal plane, to the player feet + a tiny offset and looking for a 
    /// specific ground mask. The result is stored in <c>hitInfo</c></summary>
    private void GroundCheck() {
        RaycastHit hitInfo;

        groundCheck = Physics.SphereCast(
            transform.position,
            m_characterController.radius,
            Vector3.down,
            out hitInfo,
            m_characterController.bounds.extents.y + groundOffset,
            groundMask
        );
    }

    /// <summary>
    /// Applies the jump force vertically directly to the player
    /// </summary>
    /// <param name="ctx">Context with the info of the event that triggered the jump</param>
    private void Jump(CallbackContext ctx) {
        if (groundCheck) {
            m_playerVelocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        }
    }

    private void SetupCallbacks() {
        // gameActions.Player.Jump.performed += Jump;

        gameActions.Player.Walk.performed += (ctx) => m_walking = true;
        gameActions.Player.Walk.canceled += (ctx) => m_walking = false;
    }

    private void TeardownCallbacks() {
        // gameActions.Player.Jump.performed -= Jump;

        gameActions.Player.Walk.performed -= (ctx) => m_walking = true;
        gameActions.Player.Walk.canceled -= (ctx) => m_walking = false;
    }
}
