using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Inventory;
using UnityEngine;

public class PlayerToolController : MonoBehaviour
{
    private PlayerController _playerController;

    private void OnQuickSlotItemAction(Vector2 mousePosition)
    {
        InventoryItem inventoryItem = Managers.Data.InventoryDataManager.GetQuickSlotItem();

        if (inventoryItem == null)
        {
            return;
        }

        Item item = Managers.Data.GameDataManager.GetItemData(inventoryItem.itemCode);

        if (item == null)
        {
            return;
        }

        var toolAction = ToolActionFactory.CreateAction(item, inventoryItem);
        if (toolAction != null && toolAction.CanExecuteTool(mousePosition, transform.position))
        {
            toolAction.ExecuteAction(mousePosition);
        }

        SetDirectionByMouse(mousePosition);

        if (IsAnimation(item.itemTypeId))
        {
            _playerController.ChangePlayerState(Define.PlayerState.Tool);
        }
    }

    private void SetDirectionByMouse(Vector2 mousePosition)
    {
        float disX = transform.position.x - mousePosition.x;
        float disY = transform.position.y - mousePosition.y;
        Define.PlayerDirection dir;

        if (Math.Abs(disX) > Math.Abs(disY))
        {
            dir = disX > 0 ? Define.PlayerDirection.Left : Define.PlayerDirection.Right;
        }
        else
        {
            dir = disY > 0 ? Define.PlayerDirection.Down : Define.PlayerDirection.Up;
        }

        _playerController.ChangePlayerDirection(dir);
    }

    private void OnMouseEvent(Define.MouseEvent mouseEvent)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePosition.x, mousePosition.y);
        switch (mouseEvent)
        {
            case Define.MouseEvent.LClick:
                OnQuickSlotItemAction(mousePos2D);
                break;

            case Define.MouseEvent.RClick:
                int mask = ~(1 << 2);
                Collider2D hitObject = Physics2D.OverlapPoint(mousePosition, mask);

                if (hitObject == null)
                {
                    return;
                }

                if (hitObject.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    interactable?.Interact(gameObject);
                }
                else if (hitObject.TryGetComponent<IPointInteractable>(out IPointInteractable pointInteractable))
                {
                    pointInteractable?.Interact(gameObject, mousePos2D);
                }

                break;
        }
    }

    private bool IsAnimation(Define.ItemType itemType)
    {
        return itemType == Define.ItemType.Hoe
            || itemType == Define.ItemType.WateringCan
            || itemType == Define.ItemType.Shovel
            || itemType == Define.ItemType.Pickaxe
            || itemType == Define.ItemType.Axe;
    }

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }
    
    private void OnEnable()
    {
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;
    }

    private void OnDisable()
    {
        Managers.Input.MouseAction -= OnMouseEvent;
    }
}
