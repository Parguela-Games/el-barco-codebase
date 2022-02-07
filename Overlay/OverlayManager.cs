using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : MonoBehaviour {
    private void Awake() {
        PlayerEvents.OnInventoryOpened += (items) => gameObject.SetActive(false);
        PlayerEvents.OnInventoryClosed += () => gameObject.SetActive(true);
    }

    private void OnDestroy() {
        PlayerEvents.OnInventoryOpened -= (items) => gameObject.SetActive(false);
        PlayerEvents.OnInventoryClosed -= () => gameObject.SetActive(true);
    }
}
