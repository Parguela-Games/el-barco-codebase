using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour {
    [SerializeField]
    [Tooltip("The amount of units the camera will move up and down")]
    float bobbingAmount = 0.05f;
    [SerializeField]
    [Tooltip("A factor to scale up or down the bobbing speed")]
    float bobbingSpeedScale = 0.9f;
    [SerializeField]
    [Tooltip("Minimum speed at which head will bob. Is used when player is idle")]
    float minBobbingSpeed = 3f;

    FPControllerCharacter m_characterController;

    // x component of the sin function. The more we increase this value, the more to the
    // right we are moving in the x axis of the sin function
    float m_bobbingX = 0f;
    float m_startingLocation;

    // Start is called before the first frame update
    void Start() {
        m_characterController = GetComponentInParent<FPControllerCharacter>();
        m_startingLocation = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update() {
        float player_speed = m_characterController.GetCurrentSpeed();

        if (player_speed <= Mathf.Epsilon) {
            player_speed = minBobbingSpeed;
        }

        // Player is moving
        m_bobbingX += player_speed * bobbingSpeedScale * Time.deltaTime;

        transform.localPosition = new Vector3(transform.localPosition.x, m_startingLocation + Mathf.Sin(m_bobbingX) * bobbingAmount, transform.localPosition.z);
    }
}
