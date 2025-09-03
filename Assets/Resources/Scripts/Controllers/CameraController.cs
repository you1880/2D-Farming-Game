using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    private AreaBound _areaBound;
    private float _halfWidth;
    private float _halfHeight;

    private void UpdateBound()
    {
        _areaBound = Managers.Area.GetCurrentAreaBorders();
    }

    private void Start()
    {
        if (_playerTransform == null)
        {
            _playerTransform = GameObject.FindWithTag("Player")?.GetComponent<Transform>();
        }
        
        UpdateBound();

        _halfHeight = Camera.main.orthographicSize;
        _halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = new Vector3(_playerTransform.position.x,
            _playerTransform.position.y, -10.0f);

        cameraPosition.x = Mathf.Clamp(cameraPosition.x, _areaBound.minX + _halfWidth, _areaBound.maxX - _halfWidth);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, _areaBound.minY + _halfHeight, _areaBound.maxY - _halfHeight);

        transform.position = cameraPosition;
    }

    private void OnEnable()
    {
        Managers.Area.OnAreaChanged -= UpdateBound;
        Managers.Area.OnAreaChanged += UpdateBound;
    }

    private void OnDisable()
    {
        Managers.Area.OnAreaChanged -= UpdateBound;
    }
}
