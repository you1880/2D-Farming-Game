using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryBox : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioTrigger _audioTrigger;

    public void Interact(GameObject caller)
    {
        if (!caller.CompareTag("Player") || !IsInteractable(caller))
        {
            return;
        }

        if (Managers.Data.InventoryDataManager.GetQuickSlotItem() == null)
        {
            if (Managers.Game.GetAndAddLastInputItem())
            {
                _audioTrigger?.PlaySound();
            }
        }
        else
        {
            if (Managers.Game.TryAddDeliveryItem())
            {
                _audioTrigger?.PlaySound();
            }
        }
    }

    public bool IsInteractable(GameObject caller)
    {
        return Util.IsReachable(transform.position, caller.transform.position);
    }
}
