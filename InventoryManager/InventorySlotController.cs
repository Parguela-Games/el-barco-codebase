using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotController : MonoBehaviour {
    /// <summary>
    /// Called when the mouse enters the GUIElement or Collider.
    /// </summary>
    public void OnMouseEnter() {
        InventoryEvents.NotifySlotHighlighted(gameObject);
    }
}
