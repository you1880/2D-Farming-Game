using System.Collections;
using System.Collections.Generic;
using Data.Tile;
using UnityEngine;

public class MainScene : BaseScene
{
    [SerializeField] private Vector3 _housePosition;
    [SerializeField] private Vector3 _cavePosition;
    [SerializeField] private GameObject _breakableTreesGroup;
    private GameObject _player;
    public override void Clear() { }

    public void PlaceChangedTiles()
    {
        foreach (var kvp in Managers.Data.TileDataManager.ChangedTiles)
        {
            Vector2Int tilePosition = kvp.Key;

            PlaceChangedTile(tilePosition);
        }
    }

    private void PlaceChangedTile(Vector2Int tilePosition)
    {
        if (!Managers.Data.TileDataManager.TryGetChangedTile(tilePosition, out GridTile tile))
        {
            return;
        }

        Vector3Int pos = new Vector3Int(tilePosition.x, tilePosition.y, 0);

        if (tile is FarmTile farmTile)
        {
            PlaceFarmTile(pos, farmTile);
        }
        else if (tile is PropTile propTile)
        {
            if (propTile.prop is Data.Prop.Chest chest)
            {
                Managers.Data.ContainerService.LoadChestContainer(chest);

                Managers.Prop.SpawnProp(new PropSpawnData
                {
                    tilePosition = pos,
                    propType = Define.PropType.Furniture,
                    furnitureType = chest.furnitureType,
                    chestId = chest.chestGuid
                });
            }
            else if (propTile.prop is Data.Prop.Furnace furnace)
            {
                
                Managers.Prop.SpawnProp(new PropSpawnData
                {
                    tilePosition = pos,
                    propType = Define.PropType.Furniture,
                    furnitureType = furnace.furnitureType,
                    isMelting = furnace.isMelting
                });
            }
        }
    }

    public override void Init()
    {
        CurrentScene = Define.SceneType.Main;

        Managers.Area.Init();
        Managers.Time.Init();
        Managers.Time.FlowTime();
        
        PlaceChangedTiles();
        PlaceTree();
        CreatePlayer();
    }

    private void PlaceFarmTile(Vector3Int pos, FarmTile farmTile)
    {
        if (farmTile.isPlowed)
        {
            Managers.Tile.PlacePlowedTile(pos);
        }
        else
        {
            Managers.Tile.RemovePlowedTile(pos);
        }

        if (farmTile.isWatered)
        {
            Managers.Tile.PlaceWateredTile(pos);
        }
        else
        {
            Managers.Tile.RemoveWateredTile(pos);
        }

        if (farmTile.plantedCrop != null && farmTile.plantedCrop.cropType != Define.CropType.None)
        {
            PlaceCrop(pos, farmTile);
        }
        else
        {
            Managers.Prop.RemovePropFromDict(pos);
        }
    }

    private void PlaceCrop(Vector3Int tilePosition, FarmTile farmTile)
    {
        if (Managers.Prop.GetPropObject(tilePosition) != null)
        {
            return;
        }

        if (!farmTile.isPlowed)
        {
            return;
        }

        Define.CropType cropType = farmTile.plantedCrop.cropType;
        int growLevel = farmTile.plantedCrop.growLevel;
        bool isDead = farmTile.plantedCrop.isDead;
        
        Managers.Prop.SpawnProp(new PropSpawnData
        {
            tilePosition = tilePosition,
            propType = Define.PropType.Crop,
            cropType = cropType,
            growLevel = growLevel,
            isDead = isDead
        });
    }

    private void CreatePlayer()
    {
        if (_player == null)
        {
            _player = Managers.Resource.Instantiate("Player/Player");
        }

        if (_player == null)
        {
            return;
        }
        

        if (Managers.Area.CurrentArea == Define.Area.FarmHouse)
        {
            _player.transform.position = _housePosition;
        }
        else if (Managers.Area.CurrentArea == Define.Area.CaveEntrance)
        {
            _player.transform.position = _cavePosition;
        }
    }

    private void PlaceTree()
    {
        if (_breakableTreesGroup == null)
        {
            return;
        }

        foreach (Transform child in _breakableTreesGroup.transform)
        {
            Vector3Int treePosition = Managers.Tile.ConvertWorldToCell(child.position);

            if (Managers.Prop.CheckTreeHarvested(treePosition))
            {
                child.gameObject.SetActive(false);
            }
        }
    }


    private void OnEnable()
    {
        Managers.Data.TileDataManager.OnTileDataChanged -= PlaceChangedTile;
        Managers.Data.TileDataManager.OnTileDataChanged += PlaceChangedTile;
    }

    private void OnDisable()
    {
        Managers.Data.TileDataManager.OnTileDataChanged -= PlaceChangedTile;
    }
}
