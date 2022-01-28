using UnityEngine;

public delegate void NotifySlotHighlighted(GameObject slot);

public static class InventoryEvents {
    public static event NotifySlotHighlighted OnSlotHighlight;

    public static void NotifySlotHighlighted(GameObject slot) {
        OnSlotHighlight?.Invoke(slot);
    }
}