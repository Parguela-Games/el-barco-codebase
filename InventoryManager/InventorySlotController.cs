using System.Collections;
using System.Collections.Generic;
using Manicomio.ActionableObjects;
using UnityEngine;

public class InventorySlotController : MonoBehaviour {
    PickableObject objectInSlot;

    /// <summary>
    /// Called when the mouse enters the GUIElement or Collider.
    /// </summary>
    public void OnMouseEnter() {
        InventoryEvents.NotifySlotHighlighted(gameObject);
    }

    /// <summary>
    /// Called when the mouse is not any longer over the GUIElement or Collider.
    /// </summary>
    public void OnMouseExit() {
        InventoryEvents.NotifySlotDehighlighted(gameObject);
    }

    public PickableObject GetObjectInSlot() => objectInSlot;

    public void SetObjectInSlot(PickableObject newObject) => objectInSlot = newObject;

    public void OnClick() {
        InventoryEvents.NotifyDetailedViewOpen(objectInSlot.GetDetailedMesh(), objectInSlot.GetDetailedMaterial());
    }
}
