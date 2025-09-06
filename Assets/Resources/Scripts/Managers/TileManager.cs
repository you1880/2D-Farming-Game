using System.Collections;
using System.Collections.Generic;
using Data.Tile;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager
{
    private Grid _grid;
    private Tilemap _grassTilemap;
    private Tilemap _canPlowFarmTilemap;
    private Tilemap _plowedFarmTilemap;
    private Tilemap _wateredFarmTilemap;
    private RuleTile _plowedFarmRuleTile;
    private RuleTile _wateredFarmRuleTile;

    public Grid Grid
    {
        get
        {
            if (_grid == null)
            {
                _grid = GameObject.FindWithTag("Grid")?.GetComponent<Grid>();
            }

            return _grid;
        }
    }

    public Tilemap GrassTilemap
        => GetTilemapFromGrid(ref _grassTilemap, "Ground/Grass");

    public Tilemap CanPlowFarmTilemap
        => GetTilemapFromGrid(ref _canPlowFarmTilemap, "Ground/Farm");

    public Tilemap PlowedFarmTilemap
        => GetTilemapFromGrid(ref _plowedFarmTilemap, "Ground/Plow");

    public Tilemap WateredFarmTilemap
        => GetTilemapFromGrid(ref _wateredFarmTilemap, "Ground/Watered");

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
        _plowedFarmRuleTile = Managers.Resource.Load<RuleTile>("ExternalAssets/Resources/Tiles/FarmTile");
        _wateredFarmRuleTile = Managers.Resource.Load<RuleTile>("ExternalAssets/Resources/Tiles/WateredTile");
    }

    private Tilemap GetTilemapFromGrid(ref Tilemap tilemap, string path)
    {
        if (tilemap)
        {
            return tilemap;
        }

        if (Grid == null)
        {
            return null;
        }

        Transform transform = Grid.transform.Find(path);
        tilemap = transform ? transform.GetComponent<Tilemap>() : null;

        return tilemap;
    }
}
