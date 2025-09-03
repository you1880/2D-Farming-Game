using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePortal : MonoBehaviour
{
    [SerializeField] protected Vector2 _movePosition;
    [SerializeField] protected Define.Area _moveArea;
    
    protected abstract void MoveArea(GameObject caller);

    public void Interact(GameObject caller)
    {
        MoveArea(caller);
    }
}
