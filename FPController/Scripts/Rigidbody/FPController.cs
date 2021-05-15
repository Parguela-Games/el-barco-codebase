using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (PlayerInput))]
public class FPController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float sprintMultiplier = 1.5f;
    [SerializeField] float walkDivider = 0.5f;
    private float m_movementMultiplier = 10f;
    private Vector3 m_movDirection;
    private Vector3 m_movSlopeDirection;
    
    [Range(0, 1)]
    [SerializeField] float airSlowRate = .2f;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 10f;

    [Header("Drag")]
    [SerializeField] float groundDrag = 7f;
    [SerializeField] float airDrag = 2f;

    [Header("Ground detection")]
    [SerializeField] LayerMask groundMask;
    private CapsuleCollider m_collider;
    private bool groundCheck = true;
    private float groundOffset = 0.1f;
    
    // Slope detection
    RaycastHit slopeHit;

    private GameActions gameActions;
    private Rigidbody m_rigidbody;

    private bool m_sprinting = false;
    private bool m_walking = false;

    private void Awake() {
        gameActions = new GameActions();
        gameActions.Player.Jump.performed += Jump;
        gameActions.Player.Sprint.performed += (ctx) => m_sprinting = true;
        gameActions.Player.Walk.performed += (ctx) => m_walking = true;
        gameActions.Player.Sprint.canceled += (ctx) => m_sprinting = false;
        gameActions.Player.Walk.canceled += (ctx) => m_walking = false;
    }

    private void OnEnable() {
        gameActions.Player.Enable();
    }
    
    private void OnDisable() {
        gameActions.Player.Disable();
    }

    private void OnDestroy() {
        gameActions.Player.Jump.performed -= Jump;
        gameActions.Player.Sprint.performed -= (ctx) => m_sprinting = true;
        gameActions.Player.Walk.performed -= (ctx) => m_walking = true;
        gameActions.Player.Sprint.canceled -= (ctx) => m_sprinting = false;
        gameActions.Player.Walk.canceled -= (ctx) => m_walking = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate() {
        GroundCheck();
        ProcessMovement();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
        ProcessDrag();
        ProcessSlope();
    }

    private void ProcessInput() {
        Vector2 readAction = gameActions.Player.Move.ReadValue<Vector2>();

        Vector3 forwardVector = transform.forward * readAction.y;
        Vector3 rightVector = transform.right * readAction.x;

        m_movDirection = forwardVector + rightVector;
    }

    private void ProcessMovement() {
        Vector3 currDirection = m_movDirection;

        if(OnSlope()) {
            currDirection = m_movSlopeDirection;
        }
        
        Vector3 resultMovement = currDirection.normalized * movementSpeed * m_movementMultiplier;

        if(!groundCheck) {
            resultMovement *= airSlowRate;
        } else {
            if(m_walking) {
                resultMovement *= walkDivider;
            } else if(m_sprinting) {
                resultMovement *= sprintMultiplier;
            }
        }

        m_rigidbody.AddForce(resultMovement, ForceMode.Acceleration);
    }

    private void ProcessDrag() {
        m_rigidbody.drag = groundCheck ? groundDrag : airDrag;
    }

    private void ProcessSlope() {
        m_movSlopeDirection = Vector3.ProjectOnPlane(m_movDirection, slopeHit.normal);
    }

    private void GroundCheck() {
        groundCheck = Physics.SphereCast(
            transform.position,
            m_collider.radius,
            Vector3.down,
            out slopeHit,
            m_collider.bounds.extents.y + groundOffset,
            groundMask
        );
    }

    private bool OnSlope() => groundCheck && slopeHit.normal != Vector3.up;

    private void Jump(InputAction.CallbackContext obj)
    {
        if(!groundCheck) return;

        m_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }
}
