using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FootstepSound : MonoBehaviour
{
    [SerializeField] public float MinInterval;
    private float _lastPlayed = -999.0f;
    Grid _grid;

    void Start()
    {
        if (_grid == null)
        {
            _grid = GameObject.Find("Grid")?.GetComponent<Grid>();
        }
    }

    private Define.SurfaceType GetSurface(Vector3 worldPos)
    {
        if (_grid == null)
        {
            return Define.SurfaceType.None;
        }

        Tilemap[] tilemaps = _grid.GetComponentsInChildren<Tilemap>(true);
        Define.SurfaceType surface = Define.SurfaceType.None;
        int priority = int.MinValue;

        foreach (Tilemap tilemap in tilemaps)
        {
            if (!tilemap.gameObject.TryGetComponent(out TilemapSurface tilemapSurface))
            {
                continue;
            }

            Vector3Int cell = tilemap.WorldToCell(worldPos);
            if (!tilemap.HasTile(cell))
            {
                continue;
            }

            int pri = tilemapSurface.priority != int.MinValue
                ? tilemapSurface.priority 
                : (tilemapSurface.TryGetComponent(out TilemapRenderer tilemapRenderer) ? tilemapRenderer.sortingOrder : 0);

            if (pri > priority)
            {
                priority = pri;
                surface = tilemapSurface.surfaceType;
            }
        }

        return surface;
    }

    public void OnFootstep()
    {
        float now = Time.time;
        if (now - _lastPlayed < MinInterval)
        {
            return;
        }

        _lastPlayed = now;

        Define.SurfaceType surface = GetSurface(transform.position);

        Define.EffectSoundType effect = surface switch
        {
            Define.SurfaceType.Road => Define.EffectSoundType.FootstepRoad,
            Define.SurfaceType.HouseFloor => Define.EffectSoundType.FootstepHouseFloor,
            Define.SurfaceType.Grass => Define.EffectSoundType.FootstepGrass,
            Define.SurfaceType.Farmland => Define.EffectSoundType.FootstepFarmland,
            _ => Define.EffectSoundType.None
        };

        SoundBus.Raise(new SoundBus.SoundEvent { effectSoundType = effect });
    }
}
