
using Manicomio.ActionableObjects;

public delegate void NotifyItemPick(PickableObject pickedItem);

public static class ItemPickEvents {
    public static event NotifyItemPick OnItemPick;

    public static void NotifyItemPick(PickableObject pickedItem) {
        OnItemPick?.Invoke(pickedItem);
    }
}