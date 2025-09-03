using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLoot : MonoBehaviour
{
    [SerializeField] private GameObject _droppedItemObject;
    [SerializeField] private DroppedItem _droppedItem;
    [SerializeField] private BoxCollider2D _boxCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !_droppedItem.CanLoot)
        {
            return;
        }

        _droppedItem?.LootItem();
    }
}
