using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casher : MonoBehaviour, IInteractable
{
    [SerializeField] private Define.ShopType _shopType;

    public void Interact(GameObject caller)
    {
        if (!IsInteractable(caller) || _shopType == Define.ShopType.None)
        {
            return;
        }

        UI_Shop ui = Managers.UI.ShowPauseUI<UI_Shop>();
        ui.InitShop(_shopType);
    }

    public bool IsInteractable(GameObject caller)
    {
        return Util.IsReachable(transform.position, caller.transform.position);
    }
}
