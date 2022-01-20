using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class HeadTilt : MonoBehaviour {
    [SerializeField]
    [Tooltip("The camera will slide horizontally when tilting, this is the max range")]
    float maxHorizontalRange = 0.5f;

    [SerializeField]
    [Tooltip("The max angle the head will tilt")]
    float maxAngle = 15f;

    [SerializeField]
    float tiltSmoothFactor = 2f;

    private GameActions gameActions;

    private Vector3 startingPosition;
    private Vector3 targetPosition;
    private Vector3 startingRotation;
    private Vector3 targetRotation;

    Vector3 m_targetLocalPosition;
    Vector3 m_remainingDistance;
    Vector3 m_traveledDistance;

    Vector3 m_targetLocalRotation;
    Vector3 m_remainingRotation;
    Vector3 m_traveledRotation;

    bool tiltStatic = false;

    private void Awake() {
        gameActions = new GameActions();

        gameActions.Player.HeadTilt.performed += OnHeadTilt;
        gameActions.Player.HeadTilt.canceled += OnHeadTiltCanceled;
    }

    private void OnEnable() {
        gameActions.Player.Enable();
    }

    private void OnDisable() {
        gameActions.Player.Disable();
    }

    private void OnDestroy() {
        gameActions.Player.HeadTilt.performed -= OnHeadTilt;
        gameActions.Player.HeadTilt.canceled -= OnHeadTiltCanceled;
    }

    private void Start() {
        // Store the starting position of the camera
        startingPosition = transform.localPosition;
        // This is the position to which the camera has to travel horizontally
        targetPosition = maxHorizontalRange * Vector3.right;

        // Store the starting rotation for restoring it later
        startingRotation = transform.localEulerAngles;
        // The camera has to rotate in the z axis to the max angle
        targetRotation = new Vector3(0f, 0f, maxAngle);

        // So far, the traveled distance is zero
        m_traveledDistance = Vector3.zero;
    }

    private void Update() {
        ProcessMovement();
        ProcessRotation();
    }

    private void OnHeadTilt(CallbackContext ctx) {
        float tiltValue = ctx.ReadValue<float>();

        SetTargetPosition(tiltValue);
        // Invert the value since we are rotating in Z axis
        SetTargetRotation(-1 * tiltValue);

        tiltStatic = false;
    }

    private void SetTargetRotation(float tiltValue) {
        // Target rotation is the starting one instead of the z component, that is the target, scaled by the amount of
        // tilting pressed
        m_targetLocalRotation = new Vector3(startingRotation.x, startingRotation.y, tiltValue * targetRotation.z);
        // The remaining rotation is the target minus where we are now
        m_remainingRotation = m_targetLocalRotation - transform.localEulerAngles;

        // These set of ifs is for ensuring that we have always positive angles
        if (Mathf.Abs(m_remainingRotation.x) > 180f) {
            m_remainingRotation.x -= Mathf.Sign(m_remainingRotation.x) * 360f;
        }

        if (Mathf.Abs(m_remainingRotation.y) > 180f) {
            m_remainingRotation.y -= Mathf.Sign(m_remainingRotation.y) * 360f;
        }

        if (Mathf.Abs(m_remainingRotation.z) > 180f) {
            m_remainingRotation.z -= Mathf.Sign(m_remainingRotation.z) * 360f;
        }

        m_traveledRotation = Vector3.zero;
    }

    private void SetTargetPosition(float tiltValue) {
        // This will set the target horizontal position
        m_targetLocalPosition = new Vector3(tiltValue * targetPosition.x, startingPosition.y, startingPosition.z);
        m_remainingDistance = m_targetLocalPosition - transform.localPosition;

        m_traveledDistance = Vector3.zero;
    }

    private void OnHeadTiltCanceled(CallbackContext ctx) {
        ResetTargetPoistion();
        ResetTargetRotation();

        tiltStatic = true;
    }

    private void ResetTargetRotation() {
        // Reset the target rotation to the original one when the user stop tilting
        m_targetLocalRotation = startingRotation;
        m_remainingRotation = m_targetLocalRotation - transform.localEulerAngles;

        if (Mathf.Abs(m_remainingRotation.x) > 180f) {
            m_remainingRotation.x -= Mathf.Sign(m_remainingRotation.x) * 360f;
        }

        if (Mathf.Abs(m_remainingRotation.y) > 180f) {
            m_remainingRotation.y -= Mathf.Sign(m_remainingRotation.y) * 360f;
        }

        if (Mathf.Abs(m_remainingRotation.z) > 180f) {
            m_remainingRotation.z -= Mathf.Sign(m_remainingRotation.z) * 360f;
        }

        m_traveledRotation = Vector3.zero;
    }

    private void ResetTargetPoistion() {
        // Resets the target position to the starting one
        m_targetLocalPosition = startingPosition;
        m_remainingDistance = m_targetLocalPosition - transform.localPosition;

        m_traveledDistance = Vector3.zero;
    }

    private void ProcessMovement() {
        // If we traveled all the distance, reset the position to the starting one only if we are returning from a tilted state
        // tiltStatic is set to true when the player stop tilting the head
        if (m_traveledDistance.magnitude >= m_remainingDistance.magnitude) {
            if (tiltStatic) transform.localPosition = startingPosition;
        } else {
            Vector3 deltaMovement = tiltSmoothFactor * Time.deltaTime * m_remainingDistance;

            // Move the camera step by step to the target position
            transform.localPosition = transform.localPosition + deltaMovement;

            // Add that traveled movement to the accumulator
            m_traveledDistance += deltaMovement;
        }
    }

    private void ProcessRotation() {
        // Same as above, but with the rotation
        if (m_traveledRotation.magnitude >= m_remainingRotation.magnitude) {
            if (tiltStatic) transform.localEulerAngles = startingRotation;
        } else {
            Vector3 deltaRotation = tiltSmoothFactor * Time.deltaTime * m_remainingRotation;

            transform.localEulerAngles = transform.localEulerAngles + deltaRotation;

            m_traveledRotation += deltaRotation;
        }
    }
}
