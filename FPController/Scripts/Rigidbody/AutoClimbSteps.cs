using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class AutoClimbSteps : MonoBehaviour
{
    [Header("Step climbing")]
    [SerializeField] Transform stepFeetRay;
    [SerializeField] Transform stepKneeRay;

    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 0.1f;

    Rigidbody m_rigidBody;

    private void Awake() {
        stepKneeRay.position = new Vector3(stepKneeRay.position.x, stepHeight, stepKneeRay.position.z);
    }

    private void Start() {
        m_rigidBody = GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate() {
        StepClimb();
    }

    private void StepClimb() {
        if (m_rigidBody.velocity.sqrMagnitude < Mathf.Pow(Mathf.Epsilon, 2)) return;

        RaycastHit lowerHit;
        RaycastHit upperHit;

        RaycastHit lower45DegreesHit;
        RaycastHit upper45DegreesHit;

        RaycastHit lowerMinus45DegreesHit;
        RaycastHit upperMinus45DegreesHit;

        if(
            (Physics.Raycast(stepFeetRay.position, transform.forward, out lowerHit, 0.1f) &&
            !Physics.Raycast(stepKneeRay.position, transform.forward, out upperHit, 0.2f)) ||

            (Physics.Raycast(stepFeetRay.position, Quaternion.Euler(0f, 45, 0f) * transform.forward, out lower45DegreesHit, 0.1f) &&
            !Physics.Raycast(stepKneeRay.position, Quaternion.Euler(0f, 45, 0f) * transform.forward, out upper45DegreesHit, 0.2f)) ||

            (Physics.Raycast(stepFeetRay.position, Quaternion.Euler(0f, -45, 0f) * transform.forward, out lowerMinus45DegreesHit, 0.1f) &&
            !Physics.Raycast(stepKneeRay.position, Quaternion.Euler(0f, -45, 0f) * transform.forward, out upperMinus45DegreesHit, 0.2f))
        ) {
            m_rigidBody.MovePosition(new Vector3(transform.position.x, transform.position.y + stepSmooth * Time.fixedDeltaTime, transform.position.z));
        }
    }
}
