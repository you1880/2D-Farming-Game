using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IToolAction
{
    int ToolRange { get; }
    bool CanExecuteTool(Vector3 mousePosition, Vector3 userPosition);
    void ExecuteAction(Vector3 mousePosition);
}
