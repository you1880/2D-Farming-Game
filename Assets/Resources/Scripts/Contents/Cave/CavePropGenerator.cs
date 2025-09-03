using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavePropGenerator : MonoBehaviour
{
    [SerializeField] private GameObject Props;
    private Dictionary<Define.PropType, (int requireMineLevel, float spawnChance)> _propsChance = new Dictionary<Define.PropType, (int requireMineLevel, float spawnChance)>
    {
        {Define.PropType.Rock, (ROCK_REQ_LEVEL, ROCK_SPAWN_CHNACE)},
        {Define.PropType.CoalOre, (COAL_REQ_LEVEL, COAL_SPAWN_CHNACE)},
        {Define.PropType.IronOre, (IRON_ORE_REQ_LEVEL, IRON_ORE_SPAWN_CHANCE)},
        {Define.PropType.GoldOre, (GOLD_ORE_REQ_LEVEL, GOLD_ORE_SPAWN_CHANCE)}
    };
    private const int UP_LADDER = -1;
    private const int DOWN_LADDER = -2;
    private const int PLAYER_SUMMON_POINT = -3;
    private const int ROCK_REQ_LEVEL = 1;
    private const int COAL_REQ_LEVEL = 1;
    private const int IRON_ORE_REQ_LEVEL = 5;
    private const int GOLD_ORE_REQ_LEVEL = 20;
    private const float ROCK_SPAWN_CHNACE = 0.08f;
    private const float COAL_SPAWN_CHNACE = 0.005f;
    private const float IRON_ORE_SPAWN_CHANCE = 0.01f;
    private const float GOLD_ORE_SPAWN_CHANCE = 0.003f;
    public Vector3Int PlayerSummonPosition { get; private set; } = Vector3Int.zero;

    public void GenerateCaveProp(bool[,] cave, int mineLevel)
    {
        ClearAllProps();
        
        int width = cave.GetLength(0);
        int height = cave.GetLength(1);

        int[,] caveProps = new int[width, height];

        SetLadderAndPlayerPos(cave, caveProps, width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < height; x++)
            {
                if (cave[x, y])
                {
                    continue;
                }

                foreach (var prop in _propsChance)
                {
                    if (caveProps[x, y] == 0 && mineLevel >= prop.Value.requireMineLevel && UnityEngine.Random.value < prop.Value.spawnChance)
                    {
                        caveProps[x, y] = (int)prop.Key;
                    }
                }
            }
        }

        PlaceCaveProps(caveProps, width, height);
    }

    public void ClearAllProps()
    {
        foreach (Transform prop in Props.transform)
        {
            Managers.Resource.Destroy(prop.gameObject);
        }
    }

    private void SetLadderAndPlayerPos(bool[,] cave, int[,] caveProps, int w, int h, int tries = 5000, int minDistance = 3)
    {
        System.Random rng = new System.Random();

        for (int t = 0; t < tries; t++)
        {
            int x1 = rng.Next(0, w);
            int y1 = rng.Next(0, h);
            int x2 = rng.Next(0, w);
            int y2 = rng.Next(0, h);

            if (!cave[x1, y1] && !cave[x1, y1 - 1] && !cave[x2, y2] && GetDistance(x1, y1, x2, y2) >= minDistance)
            {
                caveProps[x1, y1] = UP_LADDER;
                caveProps[x1, y1 - 1] = PLAYER_SUMMON_POINT;
                caveProps[x2, y2] = DOWN_LADDER;

                return;
            }
        }
    }

    private void PlaceCaveProps(int[,] caveProps, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (caveProps[x, y] == 0)
                {
                    continue;
                }

                GameObject propObj = null;

                if (caveProps[x, y] == UP_LADDER || caveProps[x, y] == DOWN_LADDER)
                {
                    propObj = Managers.Resource.Instantiate($"Prop/{Define.PropType.Ladder}", Props.transform);
                    if (propObj != null)
                    {
                        bool isUp = (caveProps[x, y] == UP_LADDER) ? true : false;
                        propObj.GetComponent<Ladder>()?.SetLadderStatus(isUp);
                    }
                }
                else if (caveProps[x, y] == PLAYER_SUMMON_POINT)
                {
                    PlayerSummonPosition = new Vector3Int(x, y);
                }
                else
                {
                    propObj = Managers.Resource.Instantiate($"Prop/{(Define.PropType)caveProps[x, y]}", Props.transform);
                }

                if (propObj != null)
                {
                    propObj.transform.position = new Vector3Int(x, y);
                }
            }
        }
    }

    private int GetDistance(int x1, int y1, int x2, int y2)
    {
        int dx = Math.Abs(x1 - x2);
        int dy = Math.Abs(y1 - y2);

        return dx + dy;
    }
    
}
