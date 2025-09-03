using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveGenerator : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _wallTilemap;
    [SerializeField] private Tilemap _backWallTilemap;
    [SerializeField] private TileBase _groundTile;
    [SerializeField] private TileBase _wallTile;
    [SerializeField] private TileBase _backWallTile;
    [SerializeField] private int _caveWidth = 29;
    [SerializeField] private int _caveHeight = 29;
    [SerializeField] private int _smoothIterations = 6;
    [SerializeField] private float _fillPercent = 0.55f;

    public bool[,] Regenerate()
    {
        int currentSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        bool[,] cave = GenerateCave(currentSeed, _caveWidth, _caveHeight);
        PaintTilemap(cave);

        return cave;
    }

    private bool[,] GenerateCave(int seed, int width, int height)
    {
        var rnd = new System.Random(seed);
        var cave = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isBorder = IsBorder(x, y, width, height);
                cave[x, y] = isBorder || (rnd.NextDouble() < _fillPercent);
            }
        }

        for (int i = 0; i < _smoothIterations; i++)
        {
            Smooth(cave);
        }

        KeepFloorRegion(cave);

        return cave;

        void Smooth(bool[,] cave)
        {
            var copiedCave = (bool[,])cave.Clone();

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int walls = CountWalls(copiedCave, x, y);
                    cave[x, y] = walls >= 5;
                }
            }
        }

        int CountWalls(bool[,] cave, int cx, int cy)
        {
            int count = 0;
            for (int yy = cy - 1; yy <= cy + 1; yy++)
            {
                for (int xx = cx - 1; xx <= cx + 1; xx++)
                {
                    if (xx == cx && yy == cy)
                    {
                        continue;
                    }

                    if (cave[xx, yy])
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        void KeepFloorRegion(bool[,] cave)
        {
            var visited = new bool[width, height];
            List<List<(int x, int y)>> regions = new List<List<(int x, int y)>>();

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    if (cave[x, y] || visited[x, y])
                    {
                        continue;
                    }

                    regions.Add(FillFlood(cave, visited, x, y));
                }
            }

            if (regions.Count <= 1)
            {
                return;
            }

            var largest = regions[0];

            foreach (var region in regions)
            {
                if (region.Count > largest.Count)
                {
                    largest = region;
                }
            }

            foreach (var region in regions)
            {
                if (ReferenceEquals(region, largest))
                {
                    continue;
                }

                foreach (var (x, y) in region)
                {
                    cave[x, y] = true;
                }
            }
        }

        List<(int, int)> FillFlood(bool[,] cave, bool[,] visited, int cx, int cy)
        {
            Queue<(int, int)> queue = new Queue<(int, int)>();
            List<(int, int)> list = new List<(int, int)>();
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            queue.Enqueue((cx, cy));
            visited[cx, cy] = true;

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                list.Add((x, y));

                for (int i = 0; i < 4; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];

                    if (nx <= 0 || ny <= 0 || nx >= width - 1 || ny >= height - 1)
                    {
                        continue;
                    }

                    if (visited[nx, ny] || cave[nx, ny])
                    {
                        continue;
                    }

                    visited[nx, ny] = true;
                    queue.Enqueue((nx, ny));
                }
            }

            return list;
        }
    }

    private void PaintTilemap(bool[,] cave)
    {
        int w = cave.GetLength(0);
        int h = cave.GetLength(1);

        _groundTilemap.ClearAllTiles();
        _wallTilemap.ClearAllTiles();
        _backWallTilemap.ClearAllTiles();

        List<Vector3Int> groundPos = new List<Vector3Int>();
        List<Vector3Int> wallPos = new List<Vector3Int>();
        List<Vector3Int> backPos = new List<Vector3Int>();
        List<TileBase> groundTiles = new List<TileBase>();
        List<TileBase> wallTiles = new List<TileBase>();
        List<TileBase> backTiles = new List<TileBase>();

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (!cave[x, y])
                {
                    groundPos.Add(pos);
                    groundTiles.Add(_groundTile);
                }
                else
                {
                    backPos.Add(pos);
                    backTiles.Add(_backWallTile);

                    if (IsBoundaryWall(cave, x, y)) 
                    {
                        wallPos.Add(pos);
                        wallTiles.Add(_wallTile);
                    }
                }
            }
        }

        if (backPos.Count > 0)
        {
            _backWallTilemap.SetTiles(backPos.ToArray(), backTiles.ToArray());
            _wallTilemap.SetTiles(wallPos.ToArray(), wallTiles.ToArray());
            _groundTilemap.SetTiles(groundPos.ToArray(), groundTiles.ToArray());
        }
    }

    private bool IsBoundaryWall(bool[,] cave, int x, int y)
    {
        if (!cave[x, y])
        {
            return false;
        }

        int[] dx4 = { 1, -1, 0, 0 };
        int[] dy4 = { 0, 0, 1, -1 };
        int[] dx8 = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy8 = { -1, -1, -1, 0, 0, 1, 1, 1 };

        int w = cave.GetLength(0);
        int h = cave.GetLength(1);

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx4[i], ny = y + dy4[i];

            if (nx < 0 || ny < 0 || nx >= w || ny >= h)
                return true;
            if (!cave[nx, ny])
                return true;
        }

        for (int i = 0; i < 8; i++)
        {
            int nx = x + dx8[i], ny = y + dy8[i];

            if (nx < 0 || ny < 0 || nx >= w || ny >= h)
                return true;
            if (!cave[nx, ny])
                return true;
        }

        return false;
    }

    private bool IsBorder(int x, int y, int width, int height)
    {
        return x == 0 || y == 0 || x == width - 1 || y == height - 1;
    }

}
