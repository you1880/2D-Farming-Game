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
}
