using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Prop, IInteractable
{
    [SerializeField] private Animator _chestAnimator;
    public enum ChestState { Close, Opening, Closing };
    private ChestState _currentChestState = ChestState.Close;
    private ChestContainer _chestContainer;
    private UI_Chest _ui;
    private AudioTrigger _audioTrigger;

    public ChestState CurrentChestState
    {
        get { return _currentChestState; }
        set
        {
            if (_currentChestState == value)
            {
                return;
            }

            _currentChestState = value;
            string animationName = GetAnimationName();

            if (string.IsNullOrEmpty(animationName) || _chestAnimator == null)
            {
                return;
            }

            _chestAnimator.Play(animationName);
        }
    }

    protected override void Init()
    {
        PropType = Define.PropType.Furniture;
        _audioTrigger = GetComponent<AudioTrigger>();
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
    
    private void OnChestClosed()
    {
        if (_ui != null)
        {
            _ui.ClosedAction -= OnChestClosed;
            _ui = null;
        }

        if (CurrentChestState == ChestState.Opening)
        {
            CurrentChestState = ChestState.Closing;
            _audioTrigger?.PlaySound(Define.EffectSoundType.ChestClose);
        }
    }

    private string GetAnimationName()
    {
        return Enum.GetName(typeof(ChestState), _currentChestState);
    }

    public override void TryBreakProp(int damage = 1)
    {
        if (Managers.Game.TryBreakProp(_objectPosition, PropType, _dropTable))
        {
            Managers.Prop.RemovePropFromDict(_objectPosition);
        }
    }

    public bool IsInteractable(GameObject caller)
    {
        return Util.IsReachable(transform.position, caller.transform.position);
    }

    public void Interact(GameObject caller)
    {
        if (!IsInteractable(caller))
        {
            return;
        }

        _ui = Managers.UI.ShowPauseUI<UI_Chest>();
        if (_ui == null)
        {
            return;
        }

        CurrentChestState = ChestState.Opening;
        _audioTrigger?.PlaySound();

        _ui.InitChestUI(_chestContainer);

        _ui.ClosedAction -= OnChestClosed;
        _ui.ClosedAction += OnChestClosed;
    }

    public void SetChestId(ChestContainer chestContainer)
    {
        _chestContainer = chestContainer;
    }

}
