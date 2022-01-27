using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : MonoBehaviour {
    private void Awake() {
        PlayerEvents.OnInventoryOpened += () => enabled = false;
        PlayerEvents.OnInventoryClosed += () => enabled = true;
    }

    private void OnDestroy() {
        PlayerEvents.OnInventoryOpened -= () => enabled = false;
        PlayerEvents.OnInventoryClosed -= () => enabled = true;
    }
}
