using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Inventory;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DayEnd : UI_Base
{
    private enum Buttons
    { 
        NextDayButton
    }

    public enum Texts
    {
        ItemQuantityText,
        SellPriceText,
        TotalCostText
    }

    public enum GameObjects
    {
        DeliveryLogs
    }

    public enum Images
    {
        ItemImage,
        ItemGradeImage
    }
    private TextMeshProUGUI _quantity;
    private TextMeshProUGUI _itemSellCost;
    private TextMeshProUGUI _total;
    private GameObject _logObject;
    private GameObject _buttonObject;
    private Image _itemImage;
    private Image _itemGradeImage;
    private int _totalCost = 0;

    private void OnNextDayButtonClicked(PointerEventData data)
    {
        Managers.Game.DayEnd();
    }

    private void BindUIElements()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
    }

    private void GetUIElements()
    {
        _quantity = GetText((int)Texts.ItemQuantityText);
        _itemSellCost = GetText((int)Texts.SellPriceText);
        _total = GetText((int)Texts.TotalCostText);
        _buttonObject = GetButton((int)Buttons.NextDayButton).gameObject;
        _logObject = GetObject((int)GameObjects.DeliveryLogs);
        _itemImage = GetImage((int)Images.ItemImage);
        _itemGradeImage = GetImage((int)Images.ItemGradeImage);
    }

    private void BindButtonEvent()
    {
        _buttonObject.BindEvent(OnNextDayButtonClicked);
    }

    private void HideOrShowElements()
    {
        List<InventoryItem> inputItems = Managers.Game.GetInputItems();

        if (inputItems.Count == 0)
        {
            _logObject.SetActive(false);
        }
        else
        {
            _buttonObject.SetActive(false);
            ShowInputItems(inputItems);
        }
    }

    private void ShowInputItems(List<InventoryItem> inputItems)
    {
        if (_itemImage == null || _itemGradeImage == null || _itemSellCost == null || _quantity == null || _total == null)
        {
            return;
        }

        StartCoroutine(SwitchInputItem(inputItems));
    }

    private IEnumerator SwitchInputItem(List<InventoryItem> inputItems)
    {
        foreach (InventoryItem inventoryItem in inputItems)
        {
            Item item = Managers.Data.GameDataManager.GetItemData(inventoryItem.itemCode);
            if (item == null)
            {
                continue;
            }

            _itemImage.sprite = Managers.Resource.LoadItemSprite(inventoryItem.itemCode);

            if (inventoryItem.itemGrade == Define.ItemGrade.None)
            {
                _itemGradeImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
            else
            {
                _itemGradeImage.sprite = Managers.Resource.LoadItemGradeSprite(inventoryItem.itemGrade);
                _itemGradeImage.color = Color.white;
            }

            _quantity.text = $"x {inventoryItem.quantity:N0}";
            
            float weight = 1.0f + ((float)inventoryItem.itemGrade * 0.5f);
            int totalSell = Mathf.RoundToInt(inventoryItem.quantity * item.sellingCost * weight);
            _totalCost += totalSell;

            _itemSellCost.text = $"{totalSell:N0}";
            _total.text = $"{_totalCost:N0}";

            yield return new WaitForSecondsRealtime(1.0f);
        }

        yield return null;

        Managers.Data.UserDataManager.WalletSerivce.AddMoney(_totalCost);
        _buttonObject.SetActive(true);
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        HideOrShowElements();
    }
}
