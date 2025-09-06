using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayEndScene : BaseScene
{
    public override void Clear() {}

    public override void Init()
    {
        CurrentScene = Define.SceneType.DayEnd;
    }
}
