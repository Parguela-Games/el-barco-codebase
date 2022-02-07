using System.Collections.Generic;
using Manicomio.ActionableObjects;

public delegate void NotifyPlayerDeactivation();
public delegate void NotifyPlayerActivation();

public delegate void NotifyInventoryOpen(List<PickableObject> items);
public delegate void NotifyInventoryClose();

public static class PlayerEvents {
    public static event NotifyPlayerDeactivation OnPlayerDeactivated;
    public static event NotifyPlayerActivation OnPlayerActivated;

    public static event NotifyInventoryOpen OnInventoryOpened;
    public static event NotifyInventoryClose OnInventoryClosed;

    public static void NotifyPlayerDeactivation() {
        OnPlayerDeactivated?.Invoke();
    }

    public static void NotifyPlayerActivation() {
        OnPlayerActivated?.Invoke();
    }

    public static void NotifyInventoryOpen(List<PickableObject> items) {
        OnPlayerDeactivated?.Invoke();
        OnInventoryOpened?.Invoke(items);
    }

    public static void NotifyInventoryClose() {
        OnPlayerActivated?.Invoke();
        OnInventoryClosed?.Invoke();
    }
}