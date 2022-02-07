using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class InventoryDetailedViewManager : MonoBehaviour {
    [SerializeField]
    Camera detailedViewCamera;
    [SerializeField]
    Transform childToRotate;
    [SerializeField]
    float rotationSpeed = 10f;

    GameActions m_gameActions;
    bool m_holdingClick = false;
    float m_objectPtich = 0f;
    float m_objectYaw = 0f;
    GameObject m_detailedObject;


    private void Awake() {
        m_gameActions = new GameActions();
        SetUpCallbacks();
        gameObject.SetActive(false);
    }

    void OnEnable() {
        m_gameActions.UI.Enable();
    }

    private void OnDisable() {
        m_gameActions.UI.Disable();
    }

    private void OnDestroy() {
        TearDownCallbacks();
    }

    private void SetUpCallbacks() {
        InventoryEvents.OnDetailedViewOpen += ActivateDetailedView;
        InventoryEvents.OnDetailedViewClose += DecativateDetailedView;
        m_gameActions.UI.Click.performed += OnClick;
        m_gameActions.UI.Click.canceled += OnClickReleased;
        m_gameActions.UI.Drag.performed += OnDrag;
        m_gameActions.UI.Exit.performed += OnDetailedViewClose;
    }

    private void TearDownCallbacks() {
        InventoryEvents.OnDetailedViewOpen -= ActivateDetailedView;
        InventoryEvents.OnDetailedViewClose -= DecativateDetailedView;
        m_gameActions.UI.Click.performed -= OnClick;
        m_gameActions.UI.Click.canceled -= OnClickReleased;
        m_gameActions.UI.Drag.performed -= OnDrag;
        m_gameActions.UI.Exit.performed -= OnDetailedViewClose;
    }

    void ActivateDetailedView(Mesh detailedMesh, Material detailedMaterial) {
        m_detailedObject = new GameObject("Detailed object");
        m_detailedObject.transform.parent = childToRotate;

        MeshRenderer detailedRenderer = m_detailedObject.AddComponent<MeshRenderer>();
        detailedRenderer.material = detailedMaterial;
        m_detailedObject.AddComponent<MeshFilter>().mesh = detailedMesh;

        Debug.Log($"Object extents {detailedRenderer.bounds.extents}");

        float targetSize = Vector3.Distance(detailedViewCamera.ViewportToWorldPoint(new Vector3(0, 0.2f, 0)), detailedViewCamera.ViewportToWorldPoint(new Vector3(0, 0.8f, 0)));
        Debug.Log($"Target size {targetSize}");

        float[] components = { detailedRenderer.bounds.size.x, detailedRenderer.bounds.size.y, detailedRenderer.bounds.size.z };

        float maxComponent = components.Max();
        Debug.Log($"Current size {maxComponent}");

        Vector3 newScale = detailedRenderer.gameObject.transform.localScale;
        newScale.x = newScale.y = newScale.z = targetSize * newScale.y / maxComponent;
        Debug.Log($"New scale {newScale}");

        detailedRenderer.gameObject.transform.localScale = newScale;

        Vector3 newCenter = detailedViewCamera.transform.InverseTransformPoint(detailedViewCamera.ViewportToWorldPoint(new Vector3(0.6f, 0.5f, 0.5f)));

        Debug.Log($"Detailed object center {newCenter}");

        Debug.Log($"New Size {detailedRenderer.bounds.size}");

        detailedRenderer.transform.localPosition = Vector3.zero;
        detailedRenderer.transform.localPosition += Vector3.down * detailedRenderer.bounds.size.y / 2;
        newCenter.z += detailedRenderer.bounds.size.z / 2;
        Vector3 randomRotation = Random.rotationUniform.eulerAngles;
        randomRotation.z = 0f;
        detailedRenderer.transform.eulerAngles = randomRotation;

        transform.localPosition = newCenter;

        gameObject.SetActive(true);
    }

    void OnDetailedViewClose(CallbackContext ctx) {
        InventoryEvents.NotifyDetailedViewClose();
    }

    void DecativateDetailedView() {
        Destroy(m_detailedObject);
        m_detailedObject = null;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() { }

    private void OnClick(CallbackContext ctx) {
        Debug.Log("Click hold");
        m_holdingClick = true;
    }

    private void OnClickReleased(CallbackContext ctx) {
        Debug.Log("Click released");
        m_holdingClick = false;
    }

    private void OnDrag(CallbackContext ctx) {
        if (m_holdingClick) {
            Vector2 delta = ctx.ReadValue<Vector2>();

            // Apply the vertical camera movement and limit it between 90 and -90 degrees
            m_objectPtich = m_objectPtich + -delta.y * Time.deltaTime * rotationSpeed;
            // Do the same thing to the horizontal movement but don't limit it cause the player can turn around
            m_objectYaw = m_objectYaw + delta.x * Time.deltaTime * rotationSpeed;

            // The camera, instead, rotates along the X and Y axes so the player can look above and sideways
            childToRotate.transform.eulerAngles = new Vector3(m_objectPtich, m_objectYaw, 0f);
        }
    }
}
