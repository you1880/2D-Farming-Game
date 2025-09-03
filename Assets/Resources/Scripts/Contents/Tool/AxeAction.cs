using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeAction : IToolAction
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

        if (prop is Tree tree)
        {
            tree.TryBreakProp();
        }
    }
}
