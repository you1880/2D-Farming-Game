using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    public void Interact(GameObject caller)
    {
        if (!caller.CompareTag("Player") || !IsInteractable(caller))
        {
            return;
        }

        UI_CraftingTable ui = Managers.UI.ShowPauseUI<UI_CraftingTable>();
        if (ui != null)
        {
            ui.SetTablePosition(Managers.Tile.ConvertWorldToCell(transform.position));
        }
    }

    public bool IsInteractable(GameObject caller)
    {
        return Util.IsReachable(transform.position, caller.transform.position);
    }
}
