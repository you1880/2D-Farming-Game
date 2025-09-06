using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : BaseScene
{
    public override void Clear() {}

    public override void Init()
    {
        Managers.Scene.LoadNextScene(Define.SceneType.Lobby);
    }
}
