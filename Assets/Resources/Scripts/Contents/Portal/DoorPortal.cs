using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPortal : BasePortal, IInteractable
{
    [SerializeField] private Door _door;

    protected override void MoveArea(GameObject caller)
    {
        if (!IsInteractable(caller))
        {
            return;
        }

        if (!Managers.Area.TeleportToOtherArea(caller, _movePosition, _moveArea))
        {
            return;
        }

        if (_door != null)
        {
            StartCoroutine(_door.OpenDoor());
        }
    }

    public bool IsInteractable(GameObject caller)
    {
        return Util.IsReachable(transform.position, caller.transform.position);
    }
}
