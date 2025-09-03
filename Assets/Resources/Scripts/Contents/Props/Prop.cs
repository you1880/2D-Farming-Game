using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Prop : MonoBehaviour
{
    private const float SHAKE_DURATION = 0.2f;
    private const float INTENSITY = 0.1f;
    protected Grid _grid;
    protected Vector3Int _objectPosition;
    protected int _objectHp = 0;
    [SerializeField] protected Data.Game.PropDropTable _dropTable = null;
    public Define.PropType PropType { get; protected set; } = Define.PropType.None;
    
    protected abstract void Init();

    protected virtual void SetObjectPosition()
    {
        if (_grid == null)
        {
            return;
        }

        _objectPosition = _grid.WorldToCell(transform.position);
        transform.localPosition = _grid.GetCellCenterWorld(_objectPosition);
    }

    protected virtual void ShakeOnHit()
    {
        StartCoroutine(ShakeProp());
    }

    protected IEnumerator ShakeProp()
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsedTime = 0.0f;
        while (elapsedTime < SHAKE_DURATION)
        {
            Vector2 offset = Random.insideUnitCircle * INTENSITY;
            transform.localPosition = originalPosition + new Vector3(offset.x, offset.y, 0.0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }

    public virtual void TryBreakProp(int damage = 1)
    {
        _objectHp -= damage;
        ShakeOnHit();

        if (_objectHp > 0)
        {
            return;
        }

        if (Managers.Game.TryBreakProp(transform.position, PropType))
        {
            Managers.Resource.Destroy(this.gameObject);
        }
    }

    void Start()
    {
        if (_grid == null)
        {
            _grid = GameObject.FindWithTag("Grid")?.GetComponent<Grid>();
        }

        SetObjectPosition();
        Init();
    }
}
