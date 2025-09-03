using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool IsInteractable(GameObject caller);
    void Interact(GameObject caller);
}

public interface IPointInteractable
{
    bool IsInteractable(GameObject caller, Vector3 mousePosition);
    void Interact(GameObject caller, Vector3 mousePosition);
}