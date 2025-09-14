using System;
using System.Collections;
using System.Collections.Generic;
using Data.Prop;
using Data.Tile;
using UnityEngine;

public class Furnace : Prop, IInteractable
{
    [SerializeField] private GameObject _offFurnace;
    [SerializeField] private GameObject _onFurnace;
    [SerializeField] private GameObject _outputInfo;
    [SerializeField] private SpriteRenderer _outputItemSpriteRenderer;
    [SerializeField] private AudioTrigger _audioTrigger;
    private Data.Prop.Furnace _currentFurnace = null;

    protected override void Init()
    {
        PropType = Define.PropType.Furniture;
        ChangeOutputSprite(true);
    }

    protected override void SetObjectPosition()
    {
        if (_grid == null)
        {
            return;
        }

        _objectPosition = Managers.Tile.ConvertWorldToCell(transform.position);
        transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y, 0.0f);
    }

    public override void TryBreakProp(int damage = 1)
    {
        if (Managers.Game.TryBreakProp(_objectPosition, PropType, _dropTable))
        {
            Managers.Prop.RemovePropFromDict(_objectPosition);
        }
    }

    public void Interact(GameObject caller)
    {
        if (!IsInteractable(caller) || _currentFurnace.isMelting)
        {
            return;
        }

        if (_currentFurnace.isProcessingDone)
        {
            CollectOutput();
        }
        else if (!_currentFurnace.isMelting)
        {
            TryStartSmelting();
        }
    }

    public bool IsInteractable(GameObject caller)
    {
        return Util.IsReachable(transform.position, caller.transform.position);
    }

    public void SetFurnaceStatus(Data.Prop.Furnace furnace)
    {
        if (furnace != null)
        {
            _currentFurnace = furnace;
        }
        else
        {
            if (!Managers.Data.TileDataManager.TryGetChangedTile(_objectPosition, out GridTile tile) && tile is PropTile { prop: Data.Prop.Furnace fur })
            {
                _currentFurnace = fur;
            }
        }

        if (_currentFurnace == null)
        {
            return;
        }

        ChangeFurnaceSprite(_currentFurnace.isMelting);
    }

    private void OnMelting()
    {
        if (_currentFurnace == null || !_currentFurnace.isMelting || _currentFurnace.isProcessingDone)
        {
            return;
        }

        int currentTime = Managers.Time.GetCurrentTimeInMinutes();
        if (currentTime >= _currentFurnace.leftTime)
        {
            _currentFurnace.isMelting = false;
            _currentFurnace.isProcessingDone = true;

            ChangeFurnaceSprite(_currentFurnace.isMelting);
            ChangeOutputSprite();
        }
    }

    private void TryStartSmelting()
    {
        if (Managers.Game.TryStartSmelting(_currentFurnace))
        {
            ChangeFurnaceSprite(_currentFurnace.isMelting);
            _audioTrigger?.PlaySound();
        }
    }

    private void CollectOutput()
    {
        if (Managers.Game.CollectOutput(_objectPosition, _currentFurnace))
        {
            ChangeOutputSprite(true);
        }
    }

    private void ChangeFurnaceSprite(bool isMelting)
    {
        _offFurnace.SetActive(!isMelting);
        _onFurnace.SetActive(isMelting);
    }

    private void ChangeOutputSprite(bool isCollected = false)
    {
        if (_currentFurnace == null || _outputInfo == null || _outputItemSpriteRenderer == null)
        {
            return;
        }

        if (isCollected)
        {
            _outputItemSpriteRenderer.sprite = null;
            _outputInfo.SetActive(false);
        }
        else
        {
            _outputInfo.SetActive(true);
            _outputItemSpriteRenderer.sprite = Managers.Resource.LoadItemSprite(_currentFurnace.outputItemCode);
        }
    }

    private void OnEnable()
    {
        Managers.Time.OnTimeChanged -= OnMelting;
        Managers.Time.OnTimeChanged += OnMelting;
    }

    private void OnDisable()
    {
        Managers.Time.OnTimeChanged -= OnMelting;
    }
}
