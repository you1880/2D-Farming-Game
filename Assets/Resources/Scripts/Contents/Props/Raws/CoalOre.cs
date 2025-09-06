using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalOre : Rock
{
    private const int COAL_ORE_HP = 3;
    protected override void Init()
    {
        _objectHp = COAL_ORE_HP;
        PropType = Define.PropType.CoalOre;
    }
}
