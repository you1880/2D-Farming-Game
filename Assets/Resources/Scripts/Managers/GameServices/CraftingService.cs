using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingService
{
    private InventoryDataManager inventoryDataManager => Managers.Data.InventoryDataManager;

    public void CraftItem(Vector3Int craftingTablePosition, int craftResultItemCode, List<Data.Game.CraftingMaterial> craftingMaterials)
    {
        if (!CheckCanCrafting(craftingMaterials))
        {
            return;
        }

        foreach (Data.Game.CraftingMaterial craftingMaterial in craftingMaterials)
        {
            inventoryDataManager.TryUseItemInventory(craftingMaterial.materialItemCode, craftingMaterial.materialQuantity);
        }

        if (!inventoryDataManager.AddItemInventory(craftResultItemCode, 1))
        {
            Managers.Prop.SpawnProp(new PropSpawnData
            {
                tilePosition = craftingTablePosition,
                propType = Define.PropType.DropItem,
                itemCode = craftResultItemCode,
                itemQuantity = 1
            });
        }
    }

    private bool CheckCanCrafting(List<Data.Game.CraftingMaterial> craftingMaterials)
    {
        List<(int itemCode, int quantity)> reqs = new List<(int itemCode, int quantity)>();

        foreach (Data.Game.CraftingMaterial craftingMaterial in craftingMaterials)
        {
            reqs.Add((craftingMaterial.materialItemCode, craftingMaterial.materialQuantity));
        }

        return inventoryDataManager.HasAtLeastItemsInInventory(reqs);
    }
}
