using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : Prop, IInteractable
{
    [SerializeField] private Sprite _downLadder;
    [SerializeField] private Sprite _upLadder;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private bool _isUp = false;

    public void SetLadderStatus(bool isUp)
    {
        _isUp = isUp;

        if (_spriteRenderer == null)
        {
            return;
        }

        if (_isUp)
        {
            _spriteRenderer.sprite = _upLadder;
        }
        else
        {
            _spriteRenderer.sprite = _downLadder;
        }
    }

    public void Interact(GameObject caller)
    {
        if (!caller.CompareTag("Player") || !IsInteractable(caller))
        {
            return;
        }

        PlayerController playerController = caller.GetComponent<PlayerController>();

        if (Managers.Area.CurrentArea == Define.Area.CaveEntrance)
        {
            Managers.Scene.LoadNextScene(Define.SceneType.Cave, playerController);
            return;
        }

        if (_isUp)
        {
            Managers.Scene.LoadNextScene(Define.SceneType.Main, playerController);
            Managers.Area.SetCurrentArea(Define.Area.CaveEntrance);
        }
        else
        {
            BaseScene scene = Managers.Scene.CurrentScene;
            if (scene is CaveScene caveScene)
            {
                caveScene.ReGenerateCave();
            }
        }
    }

    public bool IsInteractable(GameObject caller)
    {
        return Util.IsReachable(this.transform.position, caller.transform.position);
    }

    protected override void Init()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = Util.GetOrAddComponent<SpriteRenderer>(this.gameObject);
        }
    }

    /// <summary>
    /// * Do Nothing
    /// Only for using SetObjectPosition()
    /// </summary>
    protected override void ShakeOnHit() { return; }
}
