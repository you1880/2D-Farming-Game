using System.Collections;
using System.Collections.Generic;
using Data.Inventory;
using Data.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot
{
    private const string SLOT_IMAGE = "SlotItemImage";
    private const string SLOT_ITEM_GRADE = "SlotItemGradeImage";
    private const string SLOT_QUANTITY = "SlotItemQuantity";

    private Image _slotImage;
    private Image _slotGradeImage;
    private TextMeshProUGUI _slotQuantity;

    public InventorySlot(Image slotImage, Image slotGradeImage, TextMeshProUGUI slotQuantity)
    {
        _slotImage = slotImage;
        _slotGradeImage = slotGradeImage;
        _slotQuantity = slotQuantity;
    }

    public static InventorySlot CreateInventorySlot(GameObject slot)
    {
        Image slotImage = Util.FindChild<Image>(slot, SLOT_IMAGE, true);
        Image slotGradeImage = Util.FindChild<Image>(slot, SLOT_ITEM_GRADE);
        TextMeshProUGUI slotQuantity = Util.FindChild<TextMeshProUGUI>(slot, SLOT_QUANTITY);

        return new InventorySlot(slotImage, slotGradeImage, slotQuantity);
    }

    public void SetSlotItem(InventoryItem item)
    {
        if (item == null)
        {
            _slotImage.sprite = null;
            _slotImage.color = Color.clear;

            _slotGradeImage.sprite = null;
            _slotGradeImage.color = new Color(255.0f, 255.0f, 255.0f, 0.0f);

            _slotQuantity.text = "";

            return;
        }

        _slotImage.sprite = Managers.Resource.LoadItemSprite(item.itemCode);
        _slotImage.color = Color.white;

        //TODO Item Grade Image 불러오기
        // ()
        _slotQuantity.text = item.quantity > 1 ? $"{item.quantity}" : "";
    }
}

public class UI_BaseInventory : UI_Base
{
    private const string SLOT_PREFIX = "InventorySlot";
    protected const int MAX_SLOT_NUMBER = 30;
    protected const int SLOT_NOT_SELECTED = -1;
    protected const int INVALID_SLOT_ID = -2;
    protected Vector2 _dragImageOffset = new Vector2(30.0f, -30.0f);
    protected InventoryDataManager inventoryDataManager => Managers.Data.InventoryDataManager;
    protected Dictionary<int, InventorySlot> _inventorySlots = new Dictionary<int, InventorySlot>();
    protected GameObject _dragItem;
    protected RectTransform _dragItemRect;
    protected Image _dragItemImage;
    protected TextMeshProUGUI _dragItemQuantity;
    protected int _selectedSlotId = SLOT_NOT_SELECTED;

    protected int GetSlotId(string slotName, string prefix = SLOT_PREFIX)
    {
        if (slotName.StartsWith(prefix))
        {
            string numberString = slotName.Substring(prefix.Length);

            if (int.TryParse(numberString, out int slotNumber))
            {
                return slotNumber;
            }
        }

        return INVALID_SLOT_ID;
    }

    protected void SetAllSlotItem()
    {
        for (int slotId = 0; slotId < MAX_SLOT_NUMBER; slotId++)
        {
            InventoryItem inventoryItem = inventoryDataManager.GetInventoryItem(slotId);
            if (_inventorySlots.TryGetValue(slotId, out InventorySlot inventorySlot))
            {
                inventorySlot.SetSlotItem(inventoryItem);
            }
        }
    }

    protected void InitDragItem(InventoryItem item)
    {
        if (_dragItem == null || _dragItemImage == null || _dragItemQuantity == null)
        {
            return;
        }

        _dragItem.SetActive(true);
        _dragItemImage.sprite = Managers.Resource.LoadItemSprite(item.itemCode);

        if (item.quantity > 1)
        {
            _dragItemQuantity.text = $"{item.quantity}";
        }
    }

    protected void DeactivateDragItem()
    {
        if (_dragItem == null)
        {
            return;
        }

        if (!_dragItem.activeSelf)
        {
            return;
        }

        _dragItemQuantity.text = "";
        _dragItemImage.sprite = null;

        _dragItem.SetActive(false);
    }

    protected void MoveUIElemet()
    {
        if (_dragItem != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Managers.UI.Root.transform as RectTransform,
                Input.mousePosition,
                Camera.main,
                out Vector2 localPoint);

            _dragItemRect.anchoredPosition = localPoint;
        }
    }

    protected void UpdateInventoryUI(int slotId)
    {
        if (!_inventorySlots.TryGetValue(slotId, out InventorySlot slot))
        {
            return;
        }

        slot.SetSlotItem(inventoryDataManager.GetInventoryItem(slotId));
    }

    protected void SwapInventorySlot(int targetSlotId)
    {
        inventoryDataManager.SwapInventorySlot(_selectedSlotId, targetSlotId);
        DeactivateDragItem();
        _selectedSlotId = SLOT_NOT_SELECTED;
    }

    public override void Init() { }

    private void Update()
    {
        MoveUIElemet();
    }

    private void OnEnable()
    {
        inventoryDataManager.OnInventoryChanged -= UpdateInventoryUI;
        inventoryDataManager.OnInventoryChanged += UpdateInventoryUI;
    }

    private void OnDisable()
    {
        inventoryDataManager.OnInventoryChanged -= UpdateInventoryUI;
    }
}