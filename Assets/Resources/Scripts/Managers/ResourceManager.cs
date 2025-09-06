using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Data.Game;
using UnityEngine;
using UnityEngine.U2D;

public class ResourceManager
{
    private const string EXTERNAL_ASSET_ARTS_PATH = "ExternalAssets/Resources/Arts/Item/";
    private Dictionary<string, Sprite> _cropSpriteDict = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> _itemSpriteDict = new Dictionary<string, Sprite>();
    private SpriteAtlas _cropSpriteAtlas;
    private SpriteAtlas _itemSpriteAtlas;

    public void Init()
    {
        _cropSpriteAtlas = Load<SpriteAtlas>("ExternalAssets/Resources/Arts/CropSpriteAtlas");

        if (_cropSpriteAtlas != null)
        {
            _cropSpriteDict.Clear();

            Sprite[] sprites = new Sprite[_cropSpriteAtlas.spriteCount];
            _cropSpriteAtlas.GetSprites(sprites);

            foreach (Sprite sprite in sprites)
            {
                _cropSpriteDict[sprite.name.RemoveCloneFromString()] = sprite;
            }
        }

        _itemSpriteAtlas = Load<SpriteAtlas>("ExternalAssets/Resources/Arts/ItemSpriteAtlas");

        if (_itemSpriteAtlas != null)
        {
            _itemSpriteDict.Clear();

            Sprite[] sprites = new Sprite[_itemSpriteAtlas.spriteCount];
            _itemSpriteAtlas.GetSprites(sprites);

            foreach (Sprite sprite in sprites)
            {
                _itemSpriteDict[sprite.name.RemoveCloneFromString()] = sprite;
            }
        }
    }

    public T Load<T>(string path) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log($"Invalid Path : {path}");
            return null;
        }

        return Resources.Load<T>(path);
    }

    public Sprite LoadCropSprite(Define.CropType cropType, int growLevel, bool isDead)
    {
        if (_cropSpriteDict == null)
        {
            return null;
        }

        string cropName = Enum.GetName(typeof(Define.CropType), cropType);
        string suffix = isDead ? "_Dead" : "";

        for (int level = growLevel; level >= 0; level--)
        {
            string spriteName = string.Concat(cropName, "_", level.ToString(), suffix);
            if (_cropSpriteDict.TryGetValue(spriteName, out Sprite sprite))
            {
                return sprite;
            }

            growLevel--;
        }

        return null;
    }

    public Sprite LoadItemSprite(int itemCode)
    {
        if (_itemSpriteDict == null)
        {
            return null;
        }

        Item item = Managers.Data.GameDataManager.GetItemData(itemCode);
        if (item == null)
        {
            return null;
        }

        if (_itemSpriteDict.TryGetValue(item.itemName, out Sprite sprite))
        {
            return sprite;
        }

        return null;
    }

    public Sprite LoadItemGradeSprite(Define.ItemGrade itemGrade)
    {
        if (itemGrade == Define.ItemGrade.None)
        {
            return null;
        }

        string path = string.Concat(itemGrade.ToString(), "Grade");
        if (_itemSpriteDict.TryGetValue(path, out Sprite sprite))
        {
            return sprite;
        }

        return null;
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");

        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        GameObject obj = UnityEngine.Object.Instantiate(original, parent);
        obj.name = original.name;

        return obj;
    }

    public void Destroy(GameObject obj, float time = 0.0f)
    {
        if (obj == null)
        {
            return;
        }

        UnityEngine.Object.Destroy(obj, time);
    }
}
