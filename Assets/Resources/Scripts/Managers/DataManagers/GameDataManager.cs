using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Game;
using System;
using System.IO;
using UnityEngine.UI;
using System.Linq;




#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameDataManager
{
    private GameDataSO _gameDataSO;
    public GameDataRegistry<int, Item> ItemData { get; } = new();
    public GameDataRegistry<Define.CropType, Data.Game.Crop> CropData { get; } = new();
    public GameDataRegistry<Define.PropType, Data.Game.PropDropTable> PropDropTableData { get; } = new();
    public GameDataRegistry<Define.CropType, Data.Game.CropDropTable> CropDropTableData { get; } = new();
    private GameDataRegistry<Define.ShopType, Data.Game.Shop> ShopData { get; } = new();
    private GameDataRegistry<int, Data.Game.CraftingRecipe> RecipeData { get; } = new();
    private GameDataRegistry<int, Data.Game.SmeltingRecipe> SmeltingData { get; } = new();

    public void Init()
    {
        _gameDataSO = Managers.Resource.Load<GameDataSO>("GameDataSO");
        LoadGameData();
    }

    public string GetItemSpritePath(int itemCode)
    {
        if (!ItemData.TryGet(itemCode, out Item item))
        {
            return null;
        }

        string category = Enum.GetName(typeof(Define.ItemCategory), item.itemCategoryId);
        string type = Enum.GetName(typeof(Define.ItemType), item.itemTypeId);

        if (category == null || type == null)
        {
            return null;
        }

        return $"{category}/{type}/{item.itemName}";
    }

    public Item GetItemData(int itemCode)
        => ItemData.TryGet(itemCode, out Item item) ? item : null;

    public Data.Game.Crop GetCropData(Define.CropType cropType)
        => CropData.TryGet(cropType, out Data.Game.Crop crop) ? crop : null;

    public Data.Game.CropDropTable GetCropDropTable(Define.CropType cropType)
        => CropDropTableData.TryGet(cropType, out Data.Game.CropDropTable cropDropTable) ? cropDropTable : null;

    public Data.Game.PropDropTable GetPropDropTable(Define.PropType propType)
        => PropDropTableData.TryGet(propType, out Data.Game.PropDropTable propDropTable) ? propDropTable : null;

    public Data.Game.Shop GetShopData(Define.ShopType shopType)
        => ShopData.TryGet(shopType, out Shop shop) ? shop : null;

    public Data.Game.CraftingRecipe GetRecipe(int itemCode)
        => RecipeData.TryGet(itemCode, out CraftingRecipe recipe) ? recipe : null;

    public List<Data.Game.CraftingRecipe> GetAllRecipes()
    {
        return RecipeData.Dict.Values.ToList();
    }

    public Data.Game.SmeltingRecipe GetSmeltingRecipe(int inputItemCode)
        => SmeltingData.TryGet(inputItemCode, out SmeltingRecipe smeltingRecipe) ? smeltingRecipe : null;

    private void LoadGameData()
    {
        try
        {
            ItemData.LoadData(_gameDataSO.itemDatas, item => item.itemCode);
            CropData.LoadData(_gameDataSO.cropDatas, crop => crop.cropType);
            CropDropTableData.LoadData(_gameDataSO.cropDropDatas, cropDrops => cropDrops.cropType);
            PropDropTableData.LoadData(_gameDataSO.propDropDatas, propDrops => propDrops.propType);
            ShopData.LoadData(_gameDataSO.shopDatas, shop => shop.shopType);
            RecipeData.LoadData(_gameDataSO.recipeDatas, recipe => recipe.outputItemCode);
            SmeltingData.LoadData(_gameDataSO.smeltingDatas, smelting => smelting.inputItemCode);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    #region Editor
    // Json -> ScriptableObject
    private void ConvertJsonToSO()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Data/GameData.json");
        try
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                GameDataSO dataSO = ScriptableObject.CreateInstance<GameDataSO>();
                JsonUtility.FromJsonOverwrite(json, dataSO);

                string savePath = "Assets/GameDataSO.asset";
#if UNITY_EDITOR
                AssetDatabase.CreateAsset(dataSO, savePath);
                AssetDatabase.SaveAssets();
#endif
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    #endregion
}
