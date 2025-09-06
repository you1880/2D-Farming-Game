using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casher : MonoBehaviour, IInteractable
{
    [SerializeField] private Define.ShopType _shopType;
    private UI_Shop _ui;

    public void Interact(GameObject caller)
    {
        if (!IsInteractable(caller) || _shopType == Define.ShopType.None)
        {
            return;
        }

        if (_ui == null)
        {
            _ui = Managers.UI.ShowPauseUI<UI_Shop>();

            if (_ui != null)
            {
                _ui.InitShop(_shopType);

                _ui.ClosedAction -= OnShopClosed;
                _ui.ClosedAction += OnShopClosed;
            }
        }
    }

    public bool IsInteractable(GameObject caller)
    {
        return Util.IsReachable(transform.position, caller.transform.position);
    }
    
    private void OnShopClosed()
    {
        if (_ui != null)
        {
            _ui.ClosedAction -= OnShopClosed;
            _ui = null;
        }
    }
}
