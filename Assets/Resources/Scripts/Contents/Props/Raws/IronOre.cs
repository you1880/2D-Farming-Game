using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronOre : Rock
{
    private const int IRON_ORE_HP = 5;
    protected override void Init()
    {
        _objectHp = IRON_ORE_HP;
        PropType = Define.PropType.IronOre;
    }
}
