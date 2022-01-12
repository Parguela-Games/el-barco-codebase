using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class CrouchController : MonoBehaviour {
    [Header("Dependencies")]
    [Tooltip("Collider to manipulate when crouching")]
    [SerializeField] CharacterController characterCollider;

    [Header("Crouch parameters")]
    [Tooltip("How much distance should be substracted from collider when crouching")]
    [SerializeField] float crouchHeight = 0.5f;
    [Tooltip("Factor that will divide player speed when crouching")]
    [SerializeField] float crouchDivider = 3f;
    private float currCrouchDivider = 1f;
    private float startingHeight;
    private float decrouchTime = 0.1f;

    [Header("Roof detection")]
    [Tooltip("Layer to look for when detecting a roof over the player")]
    [SerializeField] LayerMask roofMask;
    private bool m_roofCheck = true;
    private float m_roofOffset = 0.1f;
    private bool m_isDecrouching = false;

    GameActions gameActions;

    private void Awake() {
        gameActions = new GameActions();

        gameActions.Player.Crouch.performed += OnCrouchPerformed;
        gameActions.Player.Crouch.canceled += OnCrouchCanceled;
    }

    private void OnEnable() {
        gameActions.Player.Enable();
    }

    private void Start() {
        // Store the initial height of the collider for restoring it later
        startingHeight = characterCollider.height;
    }

    private void OnDisable() {
        gameActions.Player.Disable();
    }

    private void OnDestroy() {
        gameActions.Player.Crouch.performed -= OnCrouchPerformed;
        gameActions.Player.Crouch.canceled -= OnCrouchCanceled;
    }

    private void Update() {
        RoofCheck();
    }

    private void OnCrouchPerformed(CallbackContext ctx) {
        // If the player is decrouching, prevent from crouching again for avoiding strange bugs
        if (!m_isDecrouching) {
            // If We are crouching, set the speed divider
            currCrouchDivider = crouchDivider;
            // Reduce the height of the collider and let the gravity do t he rest
            characterCollider.height -= crouchHeight;
            // The collider center will be lowered down by the half of the heigh we are substracting, this will
            // lower the collider to match the lower half of the body, avoiding the texture from going through the ground
            characterCollider.center = new Vector3(characterCollider.center.x, characterCollider.center.y + crouchHeight / 2, characterCollider.center.z);
        }
    }

    private void OnCrouchCanceled(CallbackContext ctx) {
        // This checks if the player can decrouch (does not have any obstacles over the head)
        if (m_roofCheck) {
            StartCoroutine(CanDecrouch(ctx));
        } else {
            // If we can decrouch, do it and restore the divider to 1 (no modification to speed)
            currCrouchDivider = 1f;
            StartCoroutine(Decrouch());
        }
    }

    private IEnumerator CanDecrouch(CallbackContext ctx) {
        // Wait until roofCheck returns false (no roof over head)
        while (m_roofCheck) {
            yield return new WaitForSeconds(0.2f);
        }

        // Call again the routine to decrouch
        OnCrouchCanceled(ctx);
    }

    private IEnumerator Decrouch() {
        // Let others know we are decrouching
        m_isDecrouching = true;
        // How many steps will the decrouch take
        int steps = 2;
        // The full distance we have to travel to decrouch
        float fullDistance = crouchHeight / 2;
        // The collider has to be raised when decrouching, othewise the player would go through the ground
        float targetPosition = transform.position.y + fullDistance;
        // The center, instead, has be lowered, so the position of the collider is in the middle of the texture
        float targetCenter = characterCollider.center.y - fullDistance;
        // The height is the current height plus the crouch height (the amount we substracted)
        float targetHeight = characterCollider.height + crouchHeight;

        // Repeat this process until we reach the initial height (full decrouch)
        while (characterCollider.height < startingHeight) {
            // Move the player up while decrouching in equal steps to avoid going through the ground
            transform.position = new Vector3(transform.position.x, transform.position.y + fullDistance / steps, transform.position.z);
            // Lower the center in those same equal steps
            characterCollider.center = new Vector3(characterCollider.center.x, characterCollider.center.y - fullDistance / steps, characterCollider.center.z);
            // And enlarge the collider one step at a time
            characterCollider.height += crouchHeight / steps;

            // Split the decrouch time in equal steps and wait
            yield return new WaitForSeconds(decrouchTime / steps);
        }

        // This is for resetting the attributes to the initial ones, but leaving this code uncommented will cause the player to
        // flick at the end of the decrouching
        // transform.position = new Vector3(transform.position.x, targetPosition, transform.position.z);
        // characterCollider.center = new Vector3(characterCollider.center.x, targetCenter, characterCollider.center.z);
        // characterCollider.height = targetHeight;

        // Finish the routine and inform that we are finished
        m_isDecrouching = false;

        yield return null;
    }

    /// <summary>
    /// Casts a sphere from player position (center), with same radius as the player colider, 
    /// perpendicular to a horizontal plane, to the player head + a tiny offset and looking for a 
    /// specific ground mask. The result is stored in <c>hitInfo</c>
    /// </summary>
    private void RoofCheck() {
        RaycastHit hitInfo;

        m_roofCheck = Physics.SphereCast(
            transform.position,
            characterCollider.radius,
            Vector3.up,
            out hitInfo,
            characterCollider.bounds.extents.y + crouchHeight - m_roofOffset,
            roofMask
        );
    }

    public float GetCrouchDivider() => currCrouchDivider;
}
