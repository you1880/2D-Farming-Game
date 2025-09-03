using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using UnityEngine;

public class PropManager
{
    private Dictionary<Define.PropType, string> _propPrefabPath = new Dictionary<Define.PropType, string>
    {
        {Define.PropType.Crop, "Prop/Crop"},
        {Define.PropType.DropItem, "Prop/DroppedItem"},
        {Define.PropType.Furniture, "Prop/Furniture"} //TMP
    };

    private Dictionary<Vector3Int, GameObject> _propDict = new Dictionary<Vector3Int, GameObject>();
    private GameObject _props;
    private GameObject _crops;
    private GameObject _dropItems;
    private GameObject _furnitures;

    public GameObject Prop
        => GetOrCreateGroup();

    public GameObject Crops
        => GetOrCreateGroup(ref _crops, "@Crops");

    public GameObject DropItems
        => GetOrCreateGroup(ref _dropItems, "@DropItems");

    public GameObject Furnitures
        => GetOrCreateGroup(ref _furnitures, "@Furnitures");

    public GameObject SpawnProp(PropSpawnData propSpawnData)
    {
        if (propSpawnData == null || propSpawnData.propType == Define.PropType.None)
        {
            return null;
        }

        if (!_propPrefabPath.TryGetValue(propSpawnData.propType, out string path))
        {
            return null;
        }

        Vector3Int tilePosition = propSpawnData.tilePosition;
        GameObject propObject = propSpawnData.propType switch
        {
            Define.PropType.Crop => SpawnCrop(path, propSpawnData),
            Define.PropType.DropItem => SpawnDropItem(path, propSpawnData),
            Define.PropType.Furniture => SpawnFurniture(path, propSpawnData),
            _ => null
        };

        if (propObject != null)
        {
            propObject.transform.position = tilePosition;
        }

        return propObject;
    }

    public void RemovePropFromDict(Vector3Int tilePosition)
    {
        if (!_propDict.TryGetValue(tilePosition, out GameObject propObject))
        {
            return;
        }

        Managers.Resource.Destroy(propObject);
        _propDict.Remove(tilePosition);
    }

    public GameObject GetPropObject(Vector3Int tilePosition)
    {
        if (_propDict.TryGetValue(tilePosition, out GameObject propObject))
            {
                return propObject;
            }

        return null;
    }

    private GameObject GetOrCreateGroup()
    {
        if (_props != null)
        {
            return _props;
        }

        GameObject root = GameObject.Find("@Props");

        _props = root == null ? new GameObject("@Props") : root; 

        return _props;
    }

    private GameObject GetOrCreateGroup(ref GameObject group, string name)
    {

        if (group != null)
        {
            return group;
        }

        Transform transform = Prop.transform.Find(name);

        if (transform == null)
        {
            group = new GameObject(name);
            group.transform.SetParent(Prop.transform, false);
        }
        else
        {
            group = transform.gameObject;
        }

        return group;
    }

    private GameObject SpawnCrop(string path, PropSpawnData propSpawnData)
    {
        GameObject propObject = Managers.Resource.Instantiate(path, Crops.transform);

        if (propObject != null)
        {
            Define.CropType cropType = propSpawnData.cropType ?? Define.CropType.None;
            int growLevel = propSpawnData.growLevel ?? 0;
            bool isDead = propSpawnData.isDead ?? false;

            propObject.GetComponent<Crop>()?.SetCropStatus(cropType, growLevel, isDead);
            _propDict[propSpawnData.tilePosition] = propObject;
        }

        return propObject;
    }

    private GameObject SpawnDropItem(string path, PropSpawnData propSpawnData)
    {
        GameObject propObject = Managers.Resource.Instantiate(path, DropItems.transform);

        if (propObject != null)
        {
            int itemCode = propSpawnData.itemCode ?? 0;
            int quantity = propSpawnData.itemQuantity ?? 0;

            propObject.GetComponent<DroppedItem>()?.SetDropItem(itemCode, quantity);
        }

        return propObject;
    }

    private GameObject SpawnFurniture(string path, PropSpawnData propSpawnData)
    {
        Define.FurnitureType furnitureType = propSpawnData.furnitureType ?? Define.FurnitureType.None;

        if (furnitureType == Define.FurnitureType.None)
        {
            return null;
        }

        string furniturePath = $"{path}/{Enum.GetName(typeof(Define.FurnitureType), furnitureType)}";
        GameObject propObject = null;

        switch (furnitureType)
        {
            case Define.FurnitureType.Chest:
                propObject = Managers.Resource.Instantiate(furniturePath, Furnitures.transform);

                if (propObject != null)
                {
                    string id = propSpawnData.chestId ?? Managers.Data.ContainerService.CreateChestContainer();
                    ChestContainer chestContainer = Managers.Data.ContainerService.GetChestContainer(id);

                    propObject.GetComponent<Chest>()?.SetChestId(chestContainer);
                }

                break;
            case Define.FurnitureType.Furnace:
                propObject = Managers.Resource.Instantiate(furniturePath, Furnitures.transform);
                break;
        }

        if (propObject != null)
        {
            _propDict[propSpawnData.tilePosition] = propObject;
        }

        return propObject;
    }
}
