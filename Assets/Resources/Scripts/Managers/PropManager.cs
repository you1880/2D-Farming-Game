using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using UnityEngine;

public class PropManager
{
    private Dictionary<Vector3Int, GameObject> _propDict = new Dictionary<Vector3Int, GameObject>();
    private HashSet<Vector3Int> _harvestedTreeSet = new HashSet<Vector3Int>();
    private GameObject _props;
    private GameObject _crops;
    private GameObject _dropItems;
    private GameObject _furnitures;
    private GameObject _trees;

    public GameObject Prop
        => GetOrCreateGroup();

    public GameObject Crops
        => GetOrCreateGroup(ref _crops, "@Crops");

    public GameObject DropItems
        => GetOrCreateGroup(ref _dropItems, "@DropItems");

    public GameObject Furnitures
        => GetOrCreateGroup(ref _furnitures, "@Furnitures");

    public GameObject Trees
        => GetOrCreateGroup(ref _trees, "@Trees/@BreakableTrees");

    public GameObject SpawnProp(PropSpawnData propSpawnData)
    {
        if (propSpawnData == null || propSpawnData.propType == Define.PropType.None)
        {
            return null;
        }

        string path = $"Prop/{propSpawnData.propType}";

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
            Define.ItemGrade itemGrade = propSpawnData.itemGrade ?? Define.ItemGrade.None;

            propObject.GetComponent<DroppedItem>()?.SetDropItem(itemCode, quantity, itemGrade);
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

    public void RegisterTree(Vector3Int position)
    {
        if (_harvestedTreeSet.Contains(position))
        {
            return;
        }

        _harvestedTreeSet.Add(position);
    }

    public void ClearHarvestedTrees()
        => _harvestedTreeSet.Clear();

    public bool CheckTreeHarvested(Vector3Int position)
        => _harvestedTreeSet.Contains(position);
}
