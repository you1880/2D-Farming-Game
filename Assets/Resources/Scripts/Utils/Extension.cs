using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    public static void BindEvent(this GameObject obj, Action<PointerEventData> action, Define.UIEvent eventType = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(obj, action, eventType);
    }

    public static T GetOrAddComponent<T>(this GameObject obj) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(obj);
    }

    public static T SafeGetListValue<T>(this List<T> list, int index, T defaultVal = default)
    {
        if (list == null)
        {
            return defaultVal;
        }

        return (index >= 0 && index < list.Count) ? list[index] : defaultVal;
    }

    public static void SafeSetListValue<T>(this List<T> list, int index, T value)
    {
        if (list == null)
        {
            return;
        }

        if (index >= 0 && index < list.Count)
        {
            list[index] = value;
        }
    }

    public static T SafeGetArrayValue<T>(this T[] array, int index, T defaultVal = default)
    {
        if (array == null)
        {
            return defaultVal;
        }

        return (index >= 0 && index < array.Length) ? array[index] : defaultVal;
    }

    public static string RemoveCloneFromString(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        int idx = str.IndexOf("(Clone)");

        if (idx >= 0)
        {
            return str.Substring(0, idx).Trim();
        }

        return str;
    }
}
