using System.Collections;
using System.Collections.Generic;
using Data.Prop;
using UnityEngine;

public class PickaxeAction : IToolAction
{
    public int ToolRange => 1;

    public bool CanExecuteTool(Vector3 mousePosition, Vector3 userPosition)
    {
        return Util.IsToolReachable(mousePosition, userPosition, ToolRange);
    }

    public void ExecuteAction(Vector3 mousePosition)
    {
        int mask = ~(1 << 2);
        Collider2D hitObject = Physics2D.OverlapPoint(mousePosition, mask);

        if (hitObject == null)
        {
            return;
        }

        Prop prop = hitObject.GetComponent<Prop>();

        if (prop != null && prop is not (Tree or Crop))
        {
            prop.TryBreakProp();
        }

    }
}
