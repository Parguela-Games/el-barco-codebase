using Manicomio.ActionableObjects;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    List<ActionableObject> items;
    GameActions gameActions;

    private void Awake() {
        gameActions = new GameActions();
        ItemPickEvents.OnItemPick += OnItemPick;
        gameActions.Player.Inventory.performed += (ctx) => PlayerEvents.NotifyInventoryOpen();
    }

    private void OnEnable() {
        gameActions.Player.Enable();
    }

    void Start() {
        items = new List<ActionableObject>();
    }

    private void OnDisable() {
        gameActions.Player.Disable();
    }

    private void OnDestroy() {
        ItemPickEvents.OnItemPick -= OnItemPick;
        gameActions.Player.Inventory.performed -= (ctx) => PlayerEvents.NotifyInventoryOpen();
    }

    private void OnItemPick(ActionableObject pickedItem) {
        this.items.Add(pickedItem);
    }
}
