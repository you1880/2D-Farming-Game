using System.Collections;
using System.Collections.Generic;
using Data.Inventory;
using UnityEngine;

public class SmeltingService
{
    private InventoryDataManager inventoryDataManager => Managers.Data.InventoryDataManager;
    private const int COAL_ITEM_CODE = 5;
    public bool TryStartSmelting(Data.Prop.Furnace furnace)
    {
        InventoryItem inventoryItem = inventoryDataManager.GetQuickSlotItem();

        if (furnace == null || inventoryItem == null)
        {
            return false;
        }

        Data.Game.SmeltingRecipe recipe = Managers.Data.GameDataManager.GetSmeltingRecipe(inventoryItem.itemCode);

        if (recipe == null)
        {
            return false;
        }

        List<(int, int)> reqs = new List<(int, int)>
        {
            (COAL_ITEM_CODE, 1), (inventoryItem.itemCode, recipe.inputQuantity)
        };

        if (!CheckMaterials(reqs))
        {
            return false;
        }

        foreach (var req in reqs)
        {
            inventoryDataManager.TryUseItemInventory(req.Item1, req.Item2);
        }

        furnace.isMelting = true;
        furnace.meltingOreItemCode = recipe.inputItemCode;
        furnace.outputItemCode = recipe.outputItemCode;
        furnace.leftTime = GetSmeltingTime(recipe.processingHour, recipe.processingMinute);

        return true;
    }

    public bool CollectOutput(Vector3Int furnacePosition, Data.Prop.Furnace furnace)
    {
        if (furnace == null)
        {
            return false;
        }

        if (!inventoryDataManager.AddItemInventory(furnace.outputItemCode, 1))
        {
            Managers.Prop.SpawnProp(new PropSpawnData
            {
                tilePosition = furnacePosition,
                propType = Define.PropType.DropItem,
                itemCode = furnace.outputItemCode,
                itemQuantity = 1
            });
        }

        furnace.isProcessingDone = false;
        furnace.leftTime = 0;
        furnace.outputItemCode = 0;
        furnace.meltingOreItemCode = 0;

        return true;
    }

    private int GetSmeltingTime(int hour, int min)
    {
        return Managers.Time.GetCurrentTimeInMinutes() + hour * 60 + min;
    }

    private bool CheckMaterials(List<(int, int)> reqs)
    {
        return inventoryDataManager.HasAtLeastItemsInInventory(reqs);
    } 
}
