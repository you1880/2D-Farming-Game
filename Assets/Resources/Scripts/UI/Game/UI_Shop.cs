using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Inventory;
using Data.Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot
{
    private const string SLOT_ITEM_IMAGE = "ItemImage";
    private const string SLOT_ITEM_NAME = "ItemName";
    private const string SLOT_ITEM_COST = "ItemCostText";

    private Item _slotItem;
    private Image _slotImage;
    private TextMeshProUGUI _itemName;
    private TextMeshProUGUI _costText;

    public ShopSlot(Image slotImage, TextMeshProUGUI name, TextMeshProUGUI cost)
    {
        _slotImage = slotImage;
        _itemName = name;
        _costText = cost;
    }

    public static ShopSlot CreateShopSlot(GameObject slot)
    {
        Image slotImage = Util.FindChild<Image>(slot, SLOT_ITEM_IMAGE, true);
        TextMeshProUGUI itemName = Util.FindChild<TextMeshProUGUI>(slot, SLOT_ITEM_NAME);
        TextMeshProUGUI cost = Util.FindChild<TextMeshProUGUI>(slot, SLOT_ITEM_COST);

        return new ShopSlot(slotImage, itemName, cost);
    }

    public void SetSlotItem(Data.Game.Item item)
    {
        if (item == null)
        {
            return;
        }

        _slotItem = item;

        _slotImage.sprite = Managers.Resource.LoadItemSprite(item.itemCode);
        _slotImage.color = Color.white;

        _itemName.text = $"{item.itemName}";
        _costText.text = $"{item.purchaseCost:N0}";
    }

    public Item GetSlotItem()
    {
        return _slotItem;
    }
}

public class UI_Shop : UI_BaseInventory
{
    public enum Buttons
    {
        ExitButton,
    }

    public enum Texts
    {
        PlayerGoldText
    }

    public enum GameObjects
    {
        ShopContent,
        InventoryContent
    }

    private const string SHOP_SLOT_PATH = "UI/Etc/ShopSlot";
    private const string SHOP_SLOT_PREFIX = "ShopSlot";
    private const float SLOT_HEIGHTS = 110.0f;
    private enum Panel { Inventory, Shop };
    private Dictionary<int, ShopSlot> _shopSlots = new Dictionary<int, ShopSlot>();
    private PlayerData playerData => Managers.Data.UserDataManager.CurrentData;
    private IWallet playerWallet => Managers.Data.UserDataManager.WalletSerivce;
    private GameObject _shopContents;
    private GameObject _inventoryContents;
    private TextMeshProUGUI _goldText;
    private Define.ShopType _shopType;
    private bool _uiReady = false;
    private bool _shopInitalized = false;
    public event Action ClosedAction;

    public void InitShop(Define.ShopType shopType)
    {
        _shopType = shopType;

        if (_uiReady)
        {
            BuildShopSlots();
        }
    }

    private void OnInventorySlotClicked(PointerEventData data)
    {
        int slotId = GetSlotId(data.pointerClick.name);
        if (!_inventorySlots.TryGetValue(slotId, out InventorySlot slot))
        {
            return;
        }

        InventoryItem inventoryItem = inventoryDataManager.GetInventoryItem(slotId);
        if (inventoryItem == null)
        {
            return;
        }

        Item item = Managers.Data.GameDataManager.GetItemData(inventoryItem.itemCode);
        if (item == null)
        {
            return;
        }

        if (item.sellingCost == 0)
        {
            Managers.UI.ShowMessageBoxUI(MessageID.NotForSale, null, true);
            return;
        }

        int sellQuantity = (data.button == PointerEventData.InputButton.Left) ?
            1 : inventoryItem.quantity;
        int totalAmount = item.sellingCost * sellQuantity;

        if (inventoryDataManager.TryRemoveAt(slotId, sellQuantity))
        {
            playerWallet.AddMoney(totalAmount);
        }
    }

    private void OnShopSlotClicked(PointerEventData data)
    {
        int slotId = GetSlotId(data.pointerClick.name, SHOP_SLOT_PREFIX);
        if (!_shopSlots.TryGetValue(slotId, out ShopSlot slot))
        {
            return;
        }

        Item item = slot.GetSlotItem();
        if (item == null)
        {
            return;
        }

        int buyQuantity = (data.button == PointerEventData.InputButton.Left) ?
            1 : 5;
        int totalAmount = item.purchaseCost * buyQuantity;

        if (playerWallet.TrySpend(totalAmount))
        {
            inventoryDataManager.AddItemInventory(item.itemCode, buyQuantity);
        }
    }

    private void ClosedShopUI()
    {
        ClosedAction?.Invoke();
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        Managers.UI.CloseUI(this.gameObject);
    }

    private void BindUIElements()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
    }

    private void GetUIElements()
    {
        _goldText = GetText((int)Texts.PlayerGoldText);
        _shopContents = GetObject((int)GameObjects.ShopContent);
        _inventoryContents = GetObject((int)GameObjects.InventoryContent);
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
    }

    private void BuildInventorySlots()
    {
        if (_inventoryContents == null)
        {
            return;
        }

        _inventorySlots.Clear();

        foreach (Transform slotTransform in _inventoryContents.transform)
        {
            GameObject slotObject = slotTransform.gameObject;
            int slotId = GetSlotId(slotObject.name);

            InventorySlot inventorySlot = InventorySlot.CreateInventorySlot(slotObject);
            _inventorySlots[slotId] = inventorySlot;

            slotObject.BindEvent(OnInventorySlotClicked);
        }
    }

    private void BuildShopSlots()
    {
        if (_shopContents == null || _shopInitalized)
        {
            return;
        }

        List<int> shopItemList = Managers.Data.GameDataManager.GetShopData(_shopType)?.shopItemList ?? new List<int>();
        for (int i = 0; i < shopItemList.Count; i++)
        {
            Item item = Managers.Data.GameDataManager.GetItemData(shopItemList[i]);
            if (item == null)
            {
                continue;
            }

            GameObject slotObject = Managers.Resource.Instantiate(SHOP_SLOT_PATH, _shopContents.transform);
            ShopSlot shopSlot = ShopSlot.CreateShopSlot(slotObject);

            slotObject.name = $"{SHOP_SLOT_PREFIX}{i}";
            slotObject.BindEvent(OnShopSlotClicked);
            shopSlot.SetSlotItem(item);
            _shopSlots[i] = shopSlot;
        }

        RectTransform rectTransform = _shopContents.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, SLOT_HEIGHTS * shopItemList.Count);

        _shopInitalized = true;
    }

    private void SetPlayerGoldText(int gold)
    {
        _goldText.text = $"{gold:N0}";
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        BuildInventorySlots();
        SetAllSlotItem();

        _uiReady = true;
        BuildShopSlots();
        SetPlayerGoldText(playerData.gold);
    }

    private void Update() {}
    
    private void OnEnable()
    {
        inventoryDataManager.OnInventoryChanged -= UpdateInventoryUI;
        inventoryDataManager.OnInventoryChanged += UpdateInventoryUI;

        Managers.Data.UserDataManager.WalletSerivce.OnMoneyChanged -= SetPlayerGoldText;
        Managers.Data.UserDataManager.WalletSerivce.OnMoneyChanged += SetPlayerGoldText;
    }

    private void OnDisable()
    {
        inventoryDataManager.OnInventoryChanged -= UpdateInventoryUI;
        Managers.Data.UserDataManager.WalletSerivce.OnMoneyChanged -= SetPlayerGoldText;
        ClosedShopUI();
    }
}
