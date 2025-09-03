using System;
using System.Collections;
using System.Collections.Generic;
using Data.Game;
using Data.Inventory;
using Data.Player;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    private const float ANIMATION_TRANS_DURATION = 0.05f;
    [SerializeField] Sprite _manSprite;
    [SerializeField] Sprite _womanSprite;
    private Animator _animator;
    private PlayerController _playerController;
    private Define.PlayerGender _currentPlayerGender = Define.PlayerGender.None;
    private string _currentAnimationName = "";
    private int _layerIndex = 0;

    public bool SetPlayerAnimation()
    {
        string animationName = GetAnimationName();

        if (string.IsNullOrEmpty(animationName) || _currentAnimationName == animationName)
        {
            return false;
        }

        int hash = Animator.StringToHash(animationName);

        if (!_animator.HasState(_layerIndex, hash))
        {
            return false;
        }

        _animator.CrossFade(animationName, ANIMATION_TRANS_DURATION);
        _currentAnimationName = animationName;
        
        return true;
    }

    public bool SetPlayerToolAnimation()
    {
        InventoryItem inventoryItem = Managers.Data.InventoryDataManager.GetQuickSlotItem();

        if (inventoryItem == null)
        {
            return false;
        }

        Item item = Managers.Data.GameDataManager.GetItemData(inventoryItem.itemCode);

        if (item == null)
        {
            return false;
        }

        string animationName = GetToolAnimationName(item.itemTypeId);

        if (string.IsNullOrEmpty(animationName) || _currentAnimationName == animationName)
        {
            return false;
        }

        _animator.CrossFade(animationName, ANIMATION_TRANS_DURATION);
        _currentAnimationName = animationName;

        return true;
    }

    private string GetAnimationName()
    {
        if (_playerController == null)
        {
            return "";
        }

        string genderName = Enum.GetName(typeof(Define.PlayerGender), _currentPlayerGender);
        string stateName = Enum.GetName(typeof(Define.PlayerState), _playerController.PlayerState);
        string directionName = Enum.GetName(typeof(Define.PlayerDirection), _playerController.PlayerDirection);

        return $"{genderName}_{stateName}_{directionName}";
    }

    private string GetToolAnimationName(Define.ItemType itemType)
    {
        if (_playerController == null)
        {
            return "";
        }

        string genderName = Enum.GetName(typeof(Define.PlayerGender), _currentPlayerGender);
        string stateName = Enum.GetName(typeof(Define.PlayerState), _playerController.PlayerState);
        string directionName = Enum.GetName(typeof(Define.PlayerDirection), _playerController.PlayerDirection);
        string itemTypeName = Enum.GetName(typeof(Define.ItemType), itemType);

        return $"{genderName}_{stateName}_{itemTypeName}_{directionName}";
    }

    private void InitPlayerAnimation()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            return;
        }

        if (Managers.Data.UserDataManager.CurrentData != null)
        {
            _currentPlayerGender = Managers.Data.UserDataManager.CurrentData.gender;
        }

        if (_currentPlayerGender == Define.PlayerGender.Man)
        {
            spriteRenderer.sprite = _manSprite;
        }
        else
        {
            spriteRenderer.sprite = _womanSprite;
        }

        SetPlayerAnimation();
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();

        InitPlayerAnimation();
    }
}
