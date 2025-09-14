using System.Collections;
using System.Collections.Generic;
using Data.Prop;
using Data.Tile;
using UnityEngine;

public class Sprinkler : Prop
{
    public override void TryBreakProp(int damage = 1)
    {
        if (Managers.Game.TryBreakProp(_objectPosition, PropType, _dropTable))
        {
            Managers.Prop.RemovePropFromDict(_objectPosition);
        }
    }

    protected override void Init()
    {
        PropType = Define.PropType.Furniture;
    }
}
