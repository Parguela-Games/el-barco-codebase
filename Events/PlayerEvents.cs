public delegate void NotifyPlayerDeactivation();
public delegate void NotifyPlayerActivation();

public delegate void NotifyInventoryOpen();
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

    public static void NotifyInventoryOpen() {
        OnPlayerDeactivated?.Invoke();
        OnInventoryOpened?.Invoke();
    }

    public static void NotifyInventoryClose() {
        OnPlayerActivated?.Invoke();
        OnInventoryClosed?.Invoke();
    }
}