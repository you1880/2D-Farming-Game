using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetEffect : MonoBehaviour
{
    private const float MAGNET_SPEED = 3.0f;
    [SerializeField] private GameObject _droppedItemObject;
    [SerializeField] private DroppedItem _droppedItem;
    [SerializeField] private CircleCollider2D _circleCollider;
    private Transform _targetTransform;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !_droppedItem.CanLoot)
        {
            return;
        }

        _targetTransform = collision.transform;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        _targetTransform = null;
    }

    private void FixedUpdate()
    {
        if (_targetTransform != null)
        {
            _droppedItemObject.transform.position = Vector3.Lerp(_droppedItemObject.transform.position,
                _targetTransform.position, Time.deltaTime * MAGNET_SPEED);
        }
    }
}
