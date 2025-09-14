using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Inventory;
using Data.Player;
using Data.Tile;
using UnityEngine;

public class GameManager
{
    private readonly CraftingService _craftingService;
    private readonly DeliveryService _deliveryService;
    private readonly FarmingService _farmingService;
    private readonly GameFlowService _gameFlowService;
    private readonly PropService _propService;
    private readonly SmeltingService _smeltingSerivice;

    public GameManager()
    {
        _craftingService = new CraftingService();
        _deliveryService = new DeliveryService();
        _propService = new PropService();
        _smeltingSerivice = new SmeltingService();
        _farmingService = new FarmingService(_propService);
        _gameFlowService = new GameFlowService(_farmingService, _deliveryService);
    }

    public void StartGame(int saveNumber) => _gameFlowService.StartGame(saveNumber);

    public void StartNewGame(int saveNumber, string name, string farmName, Define.PlayerGender gender)
        => _gameFlowService.StartNewGame(saveNumber, name, farmName, gender);

    public void DayEnd() => _gameFlowService.DayEnd();
    public void DayStart() => _gameFlowService.DayStart();

    public bool TryBreakProp(Vector3Int tilePosition, Define.PropType propType, Data.Game.PropDropTable propDropTable = null)
        => _propService.TryBreakProp(tilePosition, propType, propDropTable);

    public bool TryBreakProp(Vector3 propPosition, Define.PropType propType, Data.Game.PropDropTable propDropTable = null)
        => _propService.TryBreakProp(propPosition, propType, propDropTable);

    public void TryHarvestCropAtPosition(Vector3 mousePosition, Vector3 userPosition)
        => _farmingService.TryHarvestCropAtPosition(mousePosition, userPosition);

    public void CraftItem(Vector3Int CraftingTablePosition, int craftResultItemCode, List<Data.Game.CraftingMaterial> craftingMaterials)
        => _craftingService.CraftItem(CraftingTablePosition, craftResultItemCode, craftingMaterials);

    public bool TryStartSmelting(Data.Prop.Furnace furnace)
        => _smeltingSerivice.TryStartSmelting(furnace);

    public bool CollectOutput(Vector3Int furnacePosition, Data.Prop.Furnace furnace)
        => _smeltingSerivice.CollectOutput(furnacePosition, furnace);

    public bool TryAddDeliveryItem()
        => _deliveryService.TryAddDeliveryItem();

    public bool GetOrAddLastInputItem()
        => _deliveryService.GetAndAddLastInputItem();

    public List<InventoryItem> GetInputItems()
        => _deliveryService.GetInputItemList();
}
