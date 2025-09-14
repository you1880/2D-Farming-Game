using System;
using System.Collections;
using System.Collections.Generic;
using Data.Inventory;
using Data.Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

        if (item.itemGrade != Define.ItemGrade.None)
        {
            _slotGradeImage.sprite = Managers.Resource.LoadItemGradeSprite(item.itemGrade);
            _slotGradeImage.color = Color.white;
        }
        else
        {
            _slotGradeImage.color = new Color(255.0f, 255.0f, 255.0f, 0.0f);
        }

        _slotQuantity.text = item.quantity > 1 ? $"{item.quantity}" : "";
    }
}

public class UI_BaseInventory : UI_Base
{
    private const string SLOT_PREFIX = "InventorySlot";
    protected const int MAX_SLOT_NUMBER = 30;
    protected const int SLOT_NOT_SELECTED = -1;
    protected const int INVALID_SLOT_ID = -2;
    protected Dictionary<int, InventorySlot> _inventorySlots = new Dictionary<int, InventorySlot>();
    protected InventoryDataManager inventoryDataManager => Managers.Data.InventoryDataManager;
    [SerializeField] protected GameObject _selectedItem;
    [SerializeField] protected RectTransform _selectedItemRect;
    [SerializeField] protected Image _selectedItemImage;
    [SerializeField] protected TextMeshProUGUI _selectedItemQuantity;
    [SerializeField] protected GameObject _itemSplit;
    [SerializeField] protected Image _splitItemImage;
    [SerializeField] protected Slider _itemSplitSlider;
    [SerializeField] protected TextMeshProUGUI _itemSplitQuantity;
    [SerializeField] protected Button _confirmButton;
    [SerializeField] protected Button _cancelButton;
    protected int _selectedSlotId = SLOT_NOT_SELECTED;
    private bool _isSplitInitailized = false;
    protected InventoryItem _splitInventoryitem = null;
    protected int _splitQuantity = 0;
    protected int _splitSlotId = SLOT_NOT_SELECTED;

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

    protected void InitDragItem(InventoryItem inventoryItem)
    {
        _selectedItem.SetActive(true);
        _selectedItemImage.sprite = Managers.Resource.LoadItemSprite(inventoryItem.itemCode);

        if (inventoryItem.quantity > 1)
        {
            _selectedItemQuantity.text = $"{inventoryItem.quantity}";
        }
    }

    protected void DeactivateDragItem()
    {
        if (!_selectedItem.activeSelf)
        {
            return;
        }

        _selectedItemQuantity.text = "";
        _selectedItemImage.sprite = null;
        _selectedSlotId = SLOT_NOT_SELECTED;
        
        _selectedItem.SetActive(false);
    }

    protected void MoveUIElemet()
    {
        if (_selectedItem.activeSelf)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Managers.UI.Root.transform as RectTransform,
                Input.mousePosition,
                Camera.main,
                out Vector2 localPoint);

            _selectedItemRect.anchoredPosition = localPoint;
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

    protected void ShowSplitUI(InventoryItem inventoryItem, int slotId)
    {
        if (inventoryItem == null || inventoryItem.quantity < 2)
        {
            return;
        }

        EnsureSplitUIInit();

        _itemSplit.SetActive(true);
        
        BindSplitUI(inventoryItem, slotId);
    }

    private void EnsureSplitUIInit()
    {
        if (_isSplitInitailized)
        {
            return;
        }

        _isSplitInitailized = true;

        _itemSplitSlider.wholeNumbers = true;
        _itemSplitSlider.onValueChanged.AddListener(OnSliderChanged);
        _confirmButton.gameObject.BindEvent(OnConfirmButtonClicked);
        _cancelButton.gameObject.BindEvent(OnCancelButtonClicked);
    }

    private void BindSplitUI(InventoryItem inventoryItem, int slotId)
    {
        _splitInventoryitem = inventoryItem;
        _splitSlotId = slotId;
        _splitQuantity = Math.Max(1, inventoryItem.quantity - 1);

        _itemSplitSlider.minValue = 1;
        _itemSplitSlider.maxValue = Math.Max(1, _splitQuantity);
        _itemSplitSlider.SetValueWithoutNotify(_splitQuantity);

        _splitItemImage.sprite = Managers.Resource.LoadItemSprite(inventoryItem.itemCode);

        OnSliderChanged(_splitQuantity);
    }

    private void OnSliderChanged(float split)
    {
        _splitQuantity = Mathf.FloorToInt(split);
        if (_itemSplitQuantity) _itemSplitQuantity.text = _splitQuantity.ToString();
    }

    protected void ClearSplitUI()
    {
        _splitInventoryitem = null;
        _splitQuantity = 0;
        _itemSplit.SetActive(false);
    }

    protected virtual void OnConfirmButtonClicked(PointerEventData data)
    {
        InventoryItem cur = inventoryDataManager.GetInventoryItem(_splitSlotId);

        if (cur == null || cur.itemCode != _splitInventoryitem.itemCode || cur.quantity < 2)
        {
            ClearSplitUI();
            return;
        }

        inventoryDataManager.TrySplitItem(_splitInventoryitem, _splitSlotId, _splitQuantity);
        ClearSplitUI();
    }

    private void OnCancelButtonClicked(PointerEventData data)
        => ClearSplitUI();

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

        _selectedItem.SetActive(false);
        _itemSplit.SetActive(false);
    }
}