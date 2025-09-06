using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject obj) where T : UnityEngine.Component
    {
        T component = obj.GetComponent<T>();

        if (component == null)
        {
            component = obj.AddComponent<T>();
        }

        return component;
    }

    public static GameObject FindChild(GameObject obj, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(obj, name, recursive);

        if (transform == null)
        {
            return null;
        }

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject obj, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (obj == null)
        {
            return null;
        }

        if (recursive)
        {
            foreach (T component in obj.GetComponentsInChildren<T>())
            {
                if (component.name == name || string.IsNullOrEmpty(name))
                {
                    return component;
                }
            }
        }
        else
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform transform = obj.transform.GetChild(i);

                if (transform.name == name || string.IsNullOrEmpty(name))
                {
                    T component = transform.GetComponent<T>();

                    if (component != null)
                    {
                        return component;
                    }
                }
            }
        }

        return null;
    }

    public static Color GetColorFromHex(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }

        return Color.white;
    }

    public static bool IsToolReachable(Vector3 mousePosition, Vector3 userPosition, int toolRange = 1)
    {
        Vector3Int tilePosition = Managers.Tile.ConvertWorldToCell(mousePosition);
        Vector3Int userTilePosition = Managers.Tile.ConvertWorldToCell(userPosition);

        int distanceX = Mathf.Abs(tilePosition.x - userTilePosition.x);
        int distanceY = Mathf.Abs(tilePosition.y - userTilePosition.y);

        return distanceX <= toolRange && distanceY <= toolRange;
    }

    public static bool IsReachable(Vector3 PropPos, Vector3 callerPosition, float range = 2)
    {
        float distanceX = Mathf.Abs(PropPos.x - callerPosition.x);
        float distanceY = Mathf.Abs(PropPos.y - callerPosition.y);

        return distanceX <= range && distanceY <= range;
    }

    public static Define.ItemGrade ItemGradeRNG(Data.Game.DropItemGradeWeights dropItemGradeWeights)
    {
        float total = dropItemGradeWeights.Total;
        if (total <= 0f)
        {
            return Define.ItemGrade.None;
        }

        float rng = UnityEngine.Random.value * total;

        if ((rng -= dropItemGradeWeights.None)   < 0.0f) return Define.ItemGrade.None;
        if ((rng -= dropItemGradeWeights.Bronze) < 0.0f) return Define.ItemGrade.Bronze;
        if ((rng -= dropItemGradeWeights.Silver) < 0.0f) return Define.ItemGrade.Silver;
        if ((rng -= dropItemGradeWeights.Gold)   < 0.0f) return Define.ItemGrade.Gold;

        return Define.ItemGrade.Amethyst;
    }
}
