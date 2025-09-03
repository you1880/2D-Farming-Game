using System.Collections;
using System.Collections.Generic;
using Data.Tile;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager
{
    private Tilemap _grassTilemap;
    private Tilemap _canPlowFarmTilemap;
    private Tilemap _plowedFarmTilemap;
    private Tilemap _wateredFarmTilemap;
    private RuleTile _plowedFarmRuleTile;
    private RuleTile _wateredFarmRuleTile;

    public Tilemap GrassTilemap
    {
        get
        {
            if (_grassTilemap == null)
            {
                _grassTilemap = GameObject.Find("Grid").transform.Find("Ground/Grass").GetComponent<Tilemap>();
            }

            return _grassTilemap;
        }
    }

    public Tilemap CanPlowFarmTilemap
    {
        get
        {
            if (_canPlowFarmTilemap == null)
            {
                _canPlowFarmTilemap = GameObject.Find("Grid").transform.Find("Ground/Farm").GetComponent<Tilemap>();
            }

            return _canPlowFarmTilemap;
        }
    }

    public Tilemap PlowedFarmTilemap
    {
        get
        {
            if (_plowedFarmTilemap == null)
            {
                _plowedFarmTilemap = GameObject.Find("Grid").transform.Find("Ground/Plow").GetComponent<Tilemap>();
            }

            return _plowedFarmTilemap;
        }
    }

    public Tilemap WateredFarmTilemap
    {
        get
        {
            if (_wateredFarmTilemap == null)
            {
                _wateredFarmTilemap = GameObject.Find("Grid").transform.Find("Ground/Watered").GetComponent<Tilemap>();
            }

            return _wateredFarmTilemap;
        }
    }

    public void PlacePlowedTile(Vector3Int tilePosition)
    {
        if (PlowedFarmTilemap == null || CanPlowFarmTilemap == null)
        {
            return;
        }

        TileBase farmTileBase = CanPlowFarmTilemap.GetTile(tilePosition);
        if (farmTileBase == null)
        {
            return;
        }

        TileBase tileBase = PlowedFarmTilemap.GetTile(tilePosition);
        if (tileBase != null)
        {
            return;
        }
        
        PlowedFarmTilemap.SetTile(tilePosition, _plowedFarmRuleTile); 
    }

    public void RemovePlowedTile(Vector3Int tilePosition)
    {
        if (PlowedFarmTilemap == null)
        {
            return;
        }

        TileBase tileBase = PlowedFarmTilemap.GetTile(tilePosition);
        if (tileBase == null)
        {
            return;
        }

        PlowedFarmTilemap.SetTile(tilePosition, null);
    }

    public void PlaceWateredTile(Vector3Int tilePosition)
    {
        if (WateredFarmTilemap == null || PlowedFarmTilemap == null)
        {
            return;
        }

        TileBase plowedTileBase = PlowedFarmTilemap.GetTile(tilePosition);
        if (plowedTileBase == null)
        {
            return;
        }

        TileBase tileBase = WateredFarmTilemap.GetTile(tilePosition);
        if (tileBase != null)
        {
            return;
        }

        WateredFarmTilemap.SetTile(tilePosition, _wateredFarmRuleTile);
    }

    public void RemoveWateredTile(Vector3Int tilePosition)
    {
        if (WateredFarmTilemap == null)
        {
            return;
        }

        TileBase tileBase = WateredFarmTilemap.GetTile(tilePosition);
        if (tileBase == null)
        {
            return;
        }

        WateredFarmTilemap.SetTile(tilePosition, null);
    }

    public Vector3Int ConvertWorldToCell(Vector3 worldPos)
    {
        return PlowedFarmTilemap != null ? PlowedFarmTilemap.WorldToCell(worldPos) : Vector3Int.RoundToInt(worldPos);
    }

    public void Init()
    {
        _plowedFarmRuleTile = Managers.Resource.Load<RuleTile>("ExternalAssets/Resources/Tile/FarmTile");
        _wateredFarmRuleTile = Managers.Resource.Load<RuleTile>("ExternalAssets/Resources/Tile/WateredTile");
    }
}
