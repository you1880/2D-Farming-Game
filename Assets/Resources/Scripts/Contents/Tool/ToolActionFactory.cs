using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Inventory;
using UnityEngine;

public static class ToolActionFactory
{
    public static IToolAction CreateAction(Item item, InventoryItem inventoryItem)
    {
        if (item.itemCategoryId == Define.ItemCategory.Furniture)
        {
            return new FurnitureAction(item, inventoryItem);
        }
        
        return item.itemTypeId switch
        {
            Define.ItemType.Hoe => new HoeAction(),
            Define.ItemType.WateringCan => new WateringCanAction(),
            Define.ItemType.Shovel => new ShovelAction(),
            Define.ItemType.Pickaxe => new PickaxeAction(),
            Define.ItemType.Axe => new AxeAction(),
            Define.ItemType.Seed => new SeedAction(item, inventoryItem),
            _ => null
        };
    }
}
