using System.Collections;
using System.Collections.Generic;
using Data.Game;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour
{
    private const float PLAYER_MOVE_SPEED = 5.0f;
    private AnimController _animationController;
    private Rigidbody2D _playerRigidbody;
    private Vector2 _moveDir;
    private Define.PlayerState _state = Define.PlayerState.Idle;
    private Define.PlayerDirection _playerDirection = Define.PlayerDirection.Down;

    public Define.PlayerState PlayerState
    {
        get { return _state; }
        private set
        {
            _state = value;

            if (_animationController == null)
            {
                return;
            }

            switch (_state)
            {
                case Define.PlayerState.Idle:
                    StopPlayerMoving();
                    _animationController.SetPlayerAnimation();
                    break;
                case Define.PlayerState.Tool:
                    StopPlayerMoving();
                    if (!_animationController.SetPlayerToolAnimation())
                    {
                        _state = Define.PlayerState.Idle;
                    }
                    break;
            }
        }
    }

    public Define.PlayerDirection PlayerDirection
    {
        get { return _playerDirection; }
        private set
        {
            _playerDirection = value;
        }
    }

    public void ChangePlayerDirection(Define.PlayerDirection playerDirection)
    {
        if (PlayerState != Define.PlayerState.Idle)
        {
            return;
        }
        
        PlayerDirection = playerDirection;

        _animationController.SetPlayerAnimation();
    }

    public void ChangePlayerState(Define.PlayerState playerState)
    {
        if (PlayerState == playerState)
        {
            return;
        }

        PlayerState = playerState;
    }

    public void StopPlayerMoving()
    {
        _playerRigidbody.velocity = Vector2.zero;
        _playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _moveDir = Vector2.zero;
    }

    private void OnKeyboardEvent(Define.KeyBoardEvent keyEvent)
    {
        if (PlayerState == Define.PlayerState.Tool)
        {
            return;
        }

        switch (keyEvent)
        {
            case Define.KeyBoardEvent.Up:
                _moveDir.y = 1;
                PlayerState = Define.PlayerState.Walk;
                PlayerDirection = Define.PlayerDirection.Up;
                break;
            case Define.KeyBoardEvent.Right:
                _moveDir.x = 1;
                PlayerState = Define.PlayerState.Walk;
                PlayerDirection = Define.PlayerDirection.Right;
                break;
            case Define.KeyBoardEvent.Down:
                _moveDir.y = -1;
                PlayerState = Define.PlayerState.Walk;
                PlayerDirection = Define.PlayerDirection.Down;
                break;
            case Define.KeyBoardEvent.Left:
                _moveDir.x = -1;
                PlayerState = Define.PlayerState.Walk;
                PlayerDirection = Define.PlayerDirection.Left;
                break;
        }

        if(_state == Define.PlayerState.Walk)
        {
            _moveDir = _moveDir.normalized;
        }
    }

    private void UpdateIdle()
    {
        StopPlayerMoving();
    }

    private void UpadteMoving()
    {
        if (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            PlayerState = Define.PlayerState.Idle;
            _animationController.SetPlayerAnimation();

            return;
        }

        _playerRigidbody.velocity = _moveDir * PLAYER_MOVE_SPEED;
        _playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (Mathf.Abs(_moveDir.x) > Mathf.Abs(_moveDir.y))
        {
            PlayerDirection = _moveDir.x > 0 ? Define.PlayerDirection.Right : Define.PlayerDirection.Left;
        }
        else if (Mathf.Abs(_moveDir.y) > 0.1f)
        {
            PlayerDirection = _moveDir.y > 0 ? Define.PlayerDirection.Up : Define.PlayerDirection.Down;
        }

        _animationController.SetPlayerAnimation();
    }

    private void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody2D>();
        _animationController = GetComponent<AnimController>();
    }

    private void Update()
    {
        switch (_state)
        {
            case Define.PlayerState.Idle:
                UpdateIdle();
                break;
            case Define.PlayerState.Walk:
                UpadteMoving();
                break;
        }
    }

    private void OnEnable()
    {
        Managers.Input.KeyAction -= OnKeyboardEvent;
        Managers.Input.KeyAction += OnKeyboardEvent;
    }

    private void OnDisable()
    {
        Managers.Input.KeyAction -= OnKeyboardEvent;
    }
}
