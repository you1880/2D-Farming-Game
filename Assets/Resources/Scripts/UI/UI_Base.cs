using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    private Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];

        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                objects[i] = Util.FindChild(gameObject, names[i], true);
            }
            else
            {
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);
            }

            if (objects[i] == null)
            {
                Debug.Log($"Failed to Bind {names[i]}");
            }
        }
    }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;

        if (_objects.TryGetValue(typeof(T), out objects) == false)
        {
            return null;
        }

        return objects[idx] as T;
    }

    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected TextMeshProUGUI GetText(int idx) { return Get<TextMeshProUGUI>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }

    public static void BindEvent(GameObject obj, Action<PointerEventData> action, Define.UIEvent eventType = Define.UIEvent.Click)
    {
        UI_EventHandler eventHandler = obj.GetOrAddComponent<UI_EventHandler>();

        switch (eventType)
        {
            case Define.UIEvent.Click:
                eventHandler.OnClickHandler -= action;
                eventHandler.OnClickHandler += action;
                break;
            case Define.UIEvent.PointerEnter:
                eventHandler.OnPointerEnterHandler -= action;
                eventHandler.OnPointerEnterHandler += action;
                break;
            case Define.UIEvent.PointerExit:
                eventHandler.OnPointerExitHandler -= action;
                eventHandler.OnPointerExitHandler += action;
                break;
        }
    }

    public virtual void Refresh() { }
    public abstract void Init();

    private void Start()
    {
        Init();
    }
}
