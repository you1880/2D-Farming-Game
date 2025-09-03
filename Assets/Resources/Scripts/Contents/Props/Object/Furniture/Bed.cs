using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    public void Interact(GameObject caller)
    {
        if (!caller.CompareTag("Player") || !IsInteractable(caller))
        {
            return;
        }

        Managers.UI.ShowMessageBoxUI(MessageID.GoNextDay, LieOnBed);
    }

    public bool IsInteractable(GameObject caller)
    {
        return Util.IsReachable(transform.position, caller.transform.position);
    }

    private void LieOnBed()
    {
        Managers.Time.SetNextDay();
    }
}
