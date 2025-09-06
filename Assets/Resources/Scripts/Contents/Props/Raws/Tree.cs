using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Prop
{
    private const int TREE_HP = 7;
    protected override void Init()
    {
        PropType = Define.PropType.Tree;
        _objectHp = TREE_HP;
    }

    public override void TryBreakProp(int damage = 1)
    {
        _objectHp -= damage;
        ShakeOnHit();

        if (_objectHp > 0)
        {
            return;
        }

        if (Managers.Game.TryBreakProp(transform.position, PropType))
        {
            Managers.Prop.RegisterTree(_objectPosition);
            Managers.Resource.Destroy(this.gameObject);
        }
    }
}
