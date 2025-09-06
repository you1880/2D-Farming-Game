using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Prop
{
    private const int ROCK_HP = 1;

    protected override void Init()
    {
        PropType = Define.PropType.Rock;
        _objectHp = ROCK_HP;
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
            GetComponent<AudioTrigger>()?.PlaySound();
            Managers.Resource.Destroy(this.gameObject);
        }
    }
}
