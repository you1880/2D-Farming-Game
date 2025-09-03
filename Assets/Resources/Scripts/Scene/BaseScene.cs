using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScene : MonoBehaviour
{
    public Define.SceneType CurrentScene = Define.SceneType.Unknown;
    
    public abstract void Init();

    public abstract void Clear();

    private void Awake()
    {
        Init();
    }
}
