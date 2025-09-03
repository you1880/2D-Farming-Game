using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDataSO", menuName = "ScriptableObjects/GameDataSO", order = 1)]
public class GameDataSO : ScriptableObject
{
    public List<Data.Game.Item> itemDatas;
    public List<Data.Game.Crop> cropDatas;
    public List<Data.Game.PropDropTable> propDropDatas;
    public List<Data.Game.CropDropTable> cropDropDatas;
    public List<Data.Game.Shop> shopDatas;
    public List<Data.Game.CraftingRecipe> recipeDatas;
    public List<Data.Game.SmeltingRecipe> smeltingDatas;

    public void LoadFromGameData(Data.Game.GameData gameData)
    {
        itemDatas = gameData.itemInfo;
        cropDatas = gameData.cropInfo;
        propDropDatas = gameData.propDropInfo;
        cropDropDatas = gameData.cropDropInfo;
        shopDatas = gameData.shopInfo;
        recipeDatas = gameData.recipeInfo;
        smeltingDatas = gameData.smeltingInfo;
    }
    
    [ContextMenu("Sort ItemDatas By ItemCode")]
    public void SortItemDatasByItemCode()
    {
        itemDatas = itemDatas.OrderBy(item => item.itemCode).ToList();
    #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
    #endif
    }
}
