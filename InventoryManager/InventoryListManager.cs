using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

enum Direction {
    UP = 1,
    DOWN = -1
}

public class InventoryListManager : MonoBehaviour {
    RectTransform[] slots;
    RectTransform contentRect;
    GridLayoutGroup contentLayoutGroup;
    RectTransform inventoryRect;
    ScrollRect inventoryScroll;

    GameActions gameActions;

    private void Awake() {
        gameActions = new GameActions();
        SetUpCallbacks();
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        TeardownCallbacks();
    }

    // Start is called before the first frame update
    void Start() {
        slots = GetComponentsInChildren<Button>().Select(button => button.GetComponent<RectTransform>()).ToArray();

        contentLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        contentRect = contentLayoutGroup.GetComponent<RectTransform>();
        inventoryRect = GetComponent<RectTransform>();
        inventoryScroll = GetComponentInChildren<ScrollRect>();
    }

    void ActivateInventory() {
        gameObject.SetActive(true);
        gameActions.UI.Enable();
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(null);
    }

    void DeactivateInventory() {
        gameObject.SetActive(false);
        gameActions.UI.Disable();
        Cursor.visible = false;
    }

    private Vector2 GetRowsAndCols() {
        // There are as many rows as distinct values for Y coordinates, since all panels are aligned
        // both horizontally and vertically.
        int rows = slots.Select(slot => slot.anchoredPosition.y).Distinct().Count();
        int cols = slots.Select(slot => slot.anchoredPosition.x).Distinct().Count();

        return new Vector2(rows, cols);
    }

    private void SelectSlot(GameObject slot) {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(slot);
    }

    private Vector2 FirstAndLastVisibleRow() {
        Vector2 rowsAndCols = GetRowsAndCols();
        int rows = ((int)rowsAndCols.x);
        int cols = ((int)rowsAndCols.y);

        // This is the top border position of the content rectangle. When user scrolls,
        // this changes accordingly. When moving up, this increases
        float contentYPosition = contentRect.anchoredPosition.y;

        int firstVisible = -1;
        int lastVisible = -1;

        // Check first slot of each row
        for (int el = 0; el < slots.Length; el += cols) {
            // Top position is ancored position (center) plus its half height. Then we have to offset it by the content
            // scrolled amount, because the ancored position is not modified when scrolling.
            float slotTopPosition = (slots[el].anchoredPosition.y + slots[el].rect.height / 2) + contentYPosition;

            // Same as top position but the height has to be substracted.
            float slotBotPosition = (slots[el].anchoredPosition.y - slots[el].rect.height / 2) + contentYPosition;

            // The first one whose top position is negative, is the first visible. 0 is the top rect border
            if (firstVisible == -1 && slotTopPosition <= Mathf.Epsilon) {
                firstVisible = el / cols;
            }

            // The first one whose bottom position is below the rectangle height (negative because 0 is top)
            // is already invisble, so we have to set the last visible to the previous one
            if (lastVisible == -1 && slotBotPosition < -inventoryRect.rect.height) {
                lastVisible = (el / cols) - 1;
            }
        }

        // If last visible is -1 it means that the last visible is the latest, since no row is below
        // rect bottom line
        return new Vector2(firstVisible, lastVisible == -1 ? rows - 1 : lastVisible);
    }

    void HandleNavigation(CallbackContext ctx) {
        // up       (0, 1)
        // down     (0, -1)
        // right    (1, 0)
        // left     (-1 , 0)
        // Vertical takes prevalence over horizontal
        Cursor.visible = false;
        Vector2 movement = ctx.ReadValue<Vector2>();

        Vector2 rowsAndCols = GetRowsAndCols();

        int nextButton = Array.FindIndex<RectTransform>(
            slots,
            (slot) => EventSystem.current.currentSelectedGameObject?.GetComponent<RectTransform>().GetInstanceID() == slot.GetInstanceID()
        );

        // Up
        if (movement.y > 0 && nextButton >= rowsAndCols.y) {
            nextButton -= (int)rowsAndCols.y;
            // Down
        } else if (movement.y < 0 && nextButton < slots.Length - rowsAndCols.y) {
            nextButton += (int)rowsAndCols.y;
            // Right
        } else if (movement.x > 0 && nextButton < slots.Length - 1) {
            nextButton += 1;
            // Left
        } else if (movement.x < 0) {
            nextButton -= 1;
        }

        nextButton = Mathf.Clamp(nextButton, 0, slots.Length);

        SelectSlot(slots[nextButton].gameObject);

        int nextButtonRow = Mathf.FloorToInt(nextButton / rowsAndCols.y);
        Vector2 visibleRows = FirstAndLastVisibleRow();

        if (nextButtonRow < visibleRows.x) {
            StartCoroutine(ScrollTo(nextButtonRow, Direction.UP));
        } else if (nextButtonRow > visibleRows.y) {
            StartCoroutine(ScrollTo(nextButtonRow, Direction.DOWN));
        }
    }

    IEnumerator ScrollTo(int row, Direction direction) {
        bool targetReached = false;

        do {
            Vector2 firstAndLastVisibleRows = FirstAndLastVisibleRow();

            targetReached = direction == Direction.DOWN ? firstAndLastVisibleRows.y == row : firstAndLastVisibleRows.x == row;

            inventoryScroll.verticalNormalizedPosition += 0.8f * Time.deltaTime * (int)direction;

            yield return new WaitForSeconds(0.000001f);
        } while (!targetReached);

        yield return null;
    }

    void SetUpCallbacks() {
        gameActions.UI.Navigate.performed += HandleNavigation;
        gameActions.UI.Exit.performed += (ctx) => PlayerEvents.NotifyInventoryClose();
        gameActions.UI.Point.performed += (ctx) => Cursor.visible = true;
        PlayerEvents.OnInventoryOpened += ActivateInventory;
        PlayerEvents.OnInventoryClosed += DeactivateInventory;
        InventoryEvents.OnSlotHighlight += (slot) => SelectSlot(slot);
    }

    void TeardownCallbacks() {
        gameActions.UI.Navigate.performed -= HandleNavigation;
        gameActions.UI.Exit.performed -= (ctx) => PlayerEvents.NotifyInventoryClose();
        gameActions.UI.Point.performed -= (ctx) => Cursor.visible = true;
        PlayerEvents.OnInventoryOpened -= ActivateInventory;
        PlayerEvents.OnInventoryClosed -= DeactivateInventory;
        InventoryEvents.OnSlotHighlight += (slot) => SelectSlot(slot);
    }
}
