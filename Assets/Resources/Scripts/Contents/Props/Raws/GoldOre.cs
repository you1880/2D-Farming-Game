using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldOre : Rock
{
    private const int GOLD_ORE_HP = 8;
    protected override void Init()
    {
        _objectHp = GOLD_ORE_HP;
        PropType = Define.PropType.GoldOre;
    }
}
