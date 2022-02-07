using Manicomio.ActionableObjects;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    List<PickableObject> items;
    GameActions gameActions;

    private void Awake() {
        gameActions = new GameActions();
        ItemPickEvents.OnItemPick += OnItemPick;
        gameActions.Player.Inventory.performed += (ctx) => PlayerEvents.NotifyInventoryOpen(items);
        PlayerEvents.OnPlayerDeactivated += () => gameActions.Player.Disable();
        PlayerEvents.OnPlayerActivated += () => gameActions.Player.Enable();
    }

    private void OnDestroy() {
        ItemPickEvents.OnItemPick -= OnItemPick;
        gameActions.Player.Inventory.performed -= (ctx) => PlayerEvents.NotifyInventoryOpen(items);
        PlayerEvents.OnPlayerDeactivated -= () => gameActions.Player.Disable();
        PlayerEvents.OnPlayerActivated -= () => gameActions.Player.Enable();
        items = null;
    }

    private void OnEnable() {
        gameActions.Player.Enable();
    }

    void Start() {
        items = new List<PickableObject>();
    }

    private void OnDisable() {
        gameActions.Player.Disable();
    }

    private void OnItemPick(PickableObject pickedItem) {
        this.items.Add(pickedItem);
    }
}
