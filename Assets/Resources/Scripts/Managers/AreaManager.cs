using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AreaBound
{
    public float minX, maxX, minY, maxY;
    public AreaBound(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }
}

public class AreaManager
{
    private Dictionary<Define.Area, AreaBound> _borders = new Dictionary<Define.Area, AreaBound>();
    private readonly AreaBound _defaultBorder = new AreaBound(0.0f, 0.0f, 0.0f, 0.0f);
    private Define.Area _currentArea = Define.Area.FarmHouse;
    private bool _isPlayerMoving = false;
    private GameObject _boundaries;
    public GameObject Boundaries
    {
        get
        {
            if (_boundaries == null)
            {
                _boundaries = GameObject.Find("@Boundaries");

                if (_boundaries == null)
                {
                    _boundaries = new GameObject { name = "@Boundaries" };
                }
            }
            return _boundaries;
        }
    }

    public Define.Area CurrentArea
    {
        get { return _currentArea; }
        private set
        {
            if (_currentArea == value)
            {
                return;
            }

            _currentArea = value;
            OnAreaChanged?.Invoke();
        }
    }

    public Action OnAreaChanged;

    public void Init()
    {
        _borders.Clear();
        CheckColliders();
    }

    public void SetCurrentArea(Define.Area area)
    {
        if (CurrentArea == area)
        {
            return;
        }

        CurrentArea = area;
    }

    public AreaBound GetCurrentAreaBorders()
    {
        if (!_borders.ContainsKey(CurrentArea))
        {
            CheckColliders();

        }

        if (!_borders.TryGetValue(CurrentArea, out AreaBound borders))
        {
            return _defaultBorder;
        }

        return borders;
    }

    public bool TeleportToOtherArea(GameObject mover, Vector2 movePosition, Define.Area area)
    {
        if (_isPlayerMoving || CurrentArea == area)
        {
            return false;
        }

        _isPlayerMoving = true;

        PlayerController controller = mover.GetComponent<PlayerController>();
        Managers.RunCoroutine(Managers.Scene.MoveSceneArea(controller,
        () =>
            {
                try
                {
                    mover.transform.position = movePosition;
                    CurrentArea = area;
                }
                finally
                {
                    _isPlayerMoving = false;
                }
            }
        ));

        return true;
    }

    private void CheckColliders()
    {
        foreach (Transform child in Boundaries.transform)
        {
            Define.Area area = GetArea(child.name);

            if (area == Define.Area.None)
            {
                continue;
            }

            BoxCollider2D collider = child.GetComponent<BoxCollider2D>();

            AreaBound borders = GetBorder(collider);
            if (!_borders.ContainsKey(area))
            {
                _borders.Add(area, borders);
            }
        }
    }

    private Define.Area GetArea(string name)
    {
        if (Enum.TryParse(name, out Define.Area area))
        {
            return area;
        }

        return Define.Area.None;
    }

    private AreaBound GetBorder(BoxCollider2D collider)
    {
        if (collider == null)
        {
            Debug.Log("Collider is Null");
            return _defaultBorder;
        }

        Bounds bound = collider.bounds;
        AreaBound borders = new AreaBound(bound.min.x, bound.max.x, bound.min.y, bound.max.y);

        return borders;
    }
}
