using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class YSorter : MonoBehaviour
{
    [SerializeField] private GameObject _ySortTarget;
    private SortingGroup _sortingGroup;
    private SpriteRenderer _targetSpriteRenderer;
    private float _checkPositionY = 0.41f;
    private int _sortingOrder = 4;

    private bool CheckYPosition()
    {
        if (_ySortTarget == null)
        {
            return false;
        }

        if (transform.position.y + _checkPositionY > _ySortTarget.transform.position.y)
        {
            return true;
        }

        return false;
    }

    private void SetSortOrder()
    {
        if (CheckYPosition())
        {
            _sortingGroup.sortingOrder = _sortingOrder - 1;
        }
        else
        {
            _sortingGroup.sortingOrder = _sortingOrder;
        }
        
    }

    void Start()
    {
        _sortingGroup = GetComponent<SortingGroup>();

        if (_ySortTarget == null)
        {
            _ySortTarget = GameObject.FindWithTag("Player");
        }

        _targetSpriteRenderer = _ySortTarget?.GetComponent<SpriteRenderer>();

        if (_targetSpriteRenderer != null)
        {
            _sortingOrder = _targetSpriteRenderer.sortingOrder;
        }
    }

    void Update()
    {
        SetSortOrder();
    }
}
