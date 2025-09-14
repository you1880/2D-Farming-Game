using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CaveScene : BaseScene
{
    [SerializeField] CaveGenerator _caveGenerator;
    [SerializeField] CavePropGenerator _cavePropGenerator;
    private GameObject _player = null;
    private int _currentMineLevel = 0;
    public event Action<int> OnLevelChanged;

    public override void Clear() { }

    public override void Init()
    {
        CurrentScene = Define.SceneType.Cave;
        Managers.Area.SetCurrentArea(Define.Area.Cave);
        ReGenerateCave();
    }

    public void ReGenerateCave()
    {
        if (_caveGenerator == null)
        {
            _caveGenerator = Util.GetOrAddComponent<CaveGenerator>(gameObject);
        }

        if (_cavePropGenerator == null)
        {
            _cavePropGenerator = Util.GetOrAddComponent<CavePropGenerator>(gameObject);
        }

        bool[,] cave = _caveGenerator?.Regenerate();

        _currentMineLevel++;
        OnLevelChanged?.Invoke(_currentMineLevel);

        _cavePropGenerator?.GenerateCaveProp(cave, _currentMineLevel);
        
        PlacePlayer();
    }

    private void PlacePlayer()
    {
        if (_player == null)
        {
            _player = Managers.Resource.Instantiate("Player/Player_Cave");
        }

        _player.transform.position = _cavePropGenerator.PlayerSummonPosition;
    }
}
