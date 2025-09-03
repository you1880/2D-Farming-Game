using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionPortal : BasePortal
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        MoveArea(collision.gameObject);
    }

    protected override void MoveArea(GameObject caller)
    {
        Managers.Area.TeleportToOtherArea(caller, _movePosition, _moveArea);
    }
}
