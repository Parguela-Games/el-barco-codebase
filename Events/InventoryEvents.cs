using UnityEngine;

public delegate void NotifySlotHighlighted(GameObject slot);
public delegate void NotifyDetailedViewOpen(Mesh detailedObj, Material detailedMaterial);
public delegate void NotifyDetailedViewClose();

public static class InventoryEvents {
    public static event NotifySlotHighlighted OnSlotHighlight;
    public static event NotifySlotHighlighted OnSlotDehighlight;
    public static event NotifyDetailedViewOpen OnDetailedViewOpen;
    public static event NotifyDetailedViewClose OnDetailedViewClose;

    public static void NotifySlotHighlighted(GameObject slot) {
        OnSlotHighlight?.Invoke(slot);
    }

    public static void NotifySlotDehighlighted(GameObject slot) {
        OnSlotDehighlight?.Invoke(slot);
    }

    public static void NotifyDetailedViewOpen(Mesh detailedMesh, Material detailedMaterial) {
        OnDetailedViewOpen?.Invoke(detailedMesh, detailedMaterial);
    }

    public static void NotifyDetailedViewClose() {
        OnDetailedViewClose?.Invoke();
    }

}