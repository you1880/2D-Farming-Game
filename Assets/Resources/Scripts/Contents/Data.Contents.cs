using System.Collections;
using System.Collections.Generic;
using Data.Inventory;
using Data.Tile;
using Newtonsoft.Json;

namespace Data.Player
{
    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        public string farmName;
        public string saveTime;
        public int gold;
        public Define.PlayerGender gender;
        public Date.Date date;
        public Inventory.PlayerInventorySave playerInventory;

        public PlayerData(string playerName, string farmName, string saveTime, int gold, Define.PlayerGender gender, Date.Date date)
        {
            this.playerName = playerName;
            this.farmName = farmName;
            this.saveTime = saveTime;
            this.gold = gold;
            this.gender = gender;
            this.date = date;
            playerInventory = new Inventory.PlayerInventorySave();
        }
    }

    [System.Serializable]
    public class PlayerTileData
    {
        public List<GridTile> tiles;

        public PlayerTileData()
        {
            tiles = new List<GridTile>();
        }
    }
}

namespace Data.Inventory
{
    [System.Serializable]
    public class PlayerInventorySave
    {
        public int inventorySize;
        public List<InventoryItemSave> inventory;

        public PlayerInventorySave()
        {
            inventory = new List<InventoryItemSave>();
        }
    }

    [System.Serializable]
    public class InventoryItem
    {
        public int itemCode;
        public int quantity;
        public int itemGrade;

        public InventoryItem(int itemCode, int quantity, int itemGrade)
        {
            this.itemCode = itemCode;
            this.quantity = quantity;
            this.itemGrade = itemGrade;
        }
    }

    public class InventoryItemSave
    {
        public int slotId;
        public int itemCode;
        public int quantity;
        public int itemGrade;

        public InventoryItemSave(int slotId, int itemCode, int quantity, int itemGrade)
        {
            this.slotId = slotId;
            this.itemCode = itemCode;
            this.quantity = quantity;
            this.itemGrade = itemGrade;
        }
    }
}

namespace Data.Date
{
    [System.Serializable]
    public class Date
    {
        public int year;
        public Define.Season season;
        public int day;

        public Date(int _year, Define.Season _season, int _day)
        {
            year = _year;
            season = _season;
            day = _day;
        }
    }
}

namespace Data.Game
{
    [System.Serializable]
    public class GameData
    {
        public List<Item> itemInfo;
        public List<Crop> cropInfo;
        public List<PropDropTable> propDropInfo;
        public List<CropDropTable> cropDropInfo;
        public List<Shop> shopInfo;
        public List<CraftingRecipe> recipeInfo;
        public List<SmeltingRecipe> smeltingInfo;

        public GameData()
        {
            itemInfo = new List<Item>();
            cropInfo = new List<Crop>();
            propDropInfo = new List<PropDropTable>();
            cropDropInfo = new List<CropDropTable>();
            shopInfo = new List<Shop>();
            recipeInfo = new List<CraftingRecipe>();
            smeltingInfo = new List<SmeltingRecipe>();
        }
    }

    [System.Serializable]
    public class Item
    {
        public int itemCode;
        public Define.ItemCategory itemCategoryId;
        public Define.ItemType itemTypeId;
        public string itemName;
        public string itemLore;
        public int maxStackSize;
        public int purchaseCost;
        public int sellingCost;

        public Item()
        {
            itemCode = 0;
            itemCategoryId = Define.ItemCategory.None;
            itemTypeId = Define.ItemType.None;
            itemName = "";
            itemLore = "";
            maxStackSize = 0;
            purchaseCost = 0;
            sellingCost = 0;
        }
    }

    [System.Serializable]
    public class Crop
    {
        public Define.CropType cropType;
        public int requireDays;
        public int repeatableRequireDays;
        public Define.Season canPlantSeason;
        public bool isRepeatable;
    }

    [System.Serializable]
    public class PlantedCrop
    {
        public Define.CropType cropType;
        public int growLevel;
        public int noWateredDays;
        public bool isDead;

        public PlantedCrop(Define.CropType _cropType, int _growLevel, int _noWateredDays, bool _isDead)
        {
            cropType = _cropType;
            growLevel = _growLevel;
            noWateredDays = _noWateredDays;
            isDead = _isDead;
        }
    }

    [System.Serializable]
    public class PropDropTable
    {
        public Define.PropType propType;
        public List<DropItem> dropItems;

        public PropDropTable()
        {
            dropItems = new List<DropItem>();
        }
    }

    [System.Serializable]
    public class CropDropTable
    {
        public Define.CropType cropType;
        public List<DropItem> dropItems;

        public CropDropTable()
        {
            dropItems = new List<DropItem>();
        }
    }

    [System.Serializable]
    public class DropItem
    {
        public int itemCode;
        public float dropChance;
        public int minDropCount;
        public int maxDropCount;
    }

    [System.Serializable]
    public class Shop
    {
        public Define.ShopType shopType;
        public List<int> shopItemList;
    }

    [System.Serializable]
    public class CraftingRecipe
    {
        public int outputItemCode;
        public List<CraftingMaterial> requireMaterials;

        public CraftingRecipe()
        {
            requireMaterials = new List<CraftingMaterial>();
        }
    }

    [System.Serializable]
    public class CraftingMaterial
    {
        public int materialItemCode;
        public int materialQuantity;
    }

    [System.Serializable]
    public class SmeltingRecipe
    {
        public int inputItemCode;
        public int inputQuantity;
        public int outputItemCode;
        public int processingHour;
        public int processingMinute;
    }
}

namespace Data.Tile
{
    [System.Serializable]
    public class GridTile
    {
        public int x;
        public int y;
    }

    [System.Serializable]
    public class FarmTile : GridTile
    {
        public bool isPlowed { get; private set; }
        public bool isWatered { get; private set; }
        public Game.PlantedCrop plantedCrop { get; private set; }

        [JsonConstructor]
        public FarmTile(int x, int y, bool isPlowed, bool isWatered, Game.PlantedCrop plantedCrop)
        {
            this.x = x;
            this.y = y;
            this.isPlowed = isPlowed;
            this.isWatered = isWatered;
            this.plantedCrop = plantedCrop;
        }
    }

    [System.Serializable]
    public class PropTile : GridTile
    {
        public Prop.Prop prop;

        public PropTile(int x, int y, Prop.Prop prop)
        {
            this.x = x;
            this.y = y;
            this.prop = prop;
        }
    }
}

namespace Data.Prop
{
    [System.Serializable]
    public class Prop
    {
        public Define.PropType propType;
    }

    [System.Serializable]
    public class Furniture : Prop
    {
        public Define.FurnitureType furnitureType;

        public Furniture()
        {
            propType = Define.PropType.Furniture;
            furnitureType = Define.FurnitureType.None;
        }

        public Furniture(Define.FurnitureType furnitureType)
        {
            propType = Define.PropType.Furniture;
            this.furnitureType = furnitureType;
        }
    }

    [System.Serializable]
    public class Chest : Furniture
    {
        public string chestGuid;
        public List<InventoryItemSave> chestItems;

        public Chest(string chestGuid)
        {
            furnitureType = Define.FurnitureType.Chest;
            this.chestGuid = chestGuid;
            chestItems = new List<InventoryItemSave>();
        }
    }

    [System.Serializable]
    public class Furnace : Furniture
    {
        public bool isMelting;
        public bool isProcessingDone;
        public int meltingOreItemCode;
        public int outputItemCode;
        public int leftTime;

        public Furnace()
        {
            furnitureType = Define.FurnitureType.Furnace;
        }
    }
}