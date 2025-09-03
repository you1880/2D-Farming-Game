using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyBind
{
    public KeyCode Key;
    public Define.InputKeyType InputType;

    public KeyBind(KeyCode key, Define.InputKeyType inputKeyType)
    {
        Key = key;
        InputType = inputKeyType;
    }
}

public class InputManager
{
    private Dictionary<Define.KeyBoardEvent, List<KeyBind>> _keyMap = new Dictionary<Define.KeyBoardEvent, List<KeyBind>>
    {
        {Define.KeyBoardEvent.Up, new List<KeyBind> {new KeyBind(KeyCode.W, Define.InputKeyType.Hold)}},
        {Define.KeyBoardEvent.Down, new List<KeyBind> {new KeyBind(KeyCode.S, Define.InputKeyType.Hold)}},
        {Define.KeyBoardEvent.Left, new List<KeyBind> {new KeyBind(KeyCode.A, Define.InputKeyType.Hold)}},
        {Define.KeyBoardEvent.Right, new List<KeyBind> {new KeyBind(KeyCode.D, Define.InputKeyType.Hold)}},
        {Define.KeyBoardEvent.Inventory, new List<KeyBind> {new KeyBind(KeyCode.E, Define.InputKeyType.Down)}},
        {Define.KeyBoardEvent.Setting, new List<KeyBind> {new KeyBind(KeyCode.Escape, Define.InputKeyType.Down)}},
        {Define.KeyBoardEvent.Num1, new List<KeyBind> {new KeyBind(KeyCode.Alpha1, Define.InputKeyType.Down)}},
        {Define.KeyBoardEvent.Num2, new List<KeyBind> {new KeyBind(KeyCode.Alpha2, Define.InputKeyType.Down)}},
        {Define.KeyBoardEvent.Num3, new List<KeyBind> {new KeyBind(KeyCode.Alpha3, Define.InputKeyType.Down)}},
        {Define.KeyBoardEvent.Num4, new List<KeyBind> {new KeyBind(KeyCode.Alpha4, Define.InputKeyType.Down)}},
        {Define.KeyBoardEvent.Num5, new List<KeyBind> {new KeyBind(KeyCode.Alpha5, Define.InputKeyType.Down)}},
        {Define.KeyBoardEvent.Num6, new List<KeyBind> {new KeyBind(KeyCode.Alpha6, Define.InputKeyType.Down)}},
        {Define.KeyBoardEvent.Num7, new List<KeyBind> {new KeyBind(KeyCode.Alpha7, Define.InputKeyType.Down)}},
        {Define.KeyBoardEvent.Num8, new List<KeyBind> {new KeyBind(KeyCode.Alpha8, Define.InputKeyType.Down)}},
        {Define.KeyBoardEvent.Num9, new List<KeyBind> {new KeyBind(KeyCode.Alpha9, Define.InputKeyType.Down)}},
    };

    private const float CLICK_DELAY = 0.5f;
    private float _pressedTime = 0.0f;
    public Action<Define.KeyBoardEvent> KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;
    
    public void OnUpdate()
    {
        if (KeyAction != null)
        {
            foreach (var keyMap in _keyMap)
            {
                foreach (var keyBind in keyMap.Value)
                {
                    bool pressed = false;

                    switch (keyBind.InputType)
                    {
                        case Define.InputKeyType.Down:
                            pressed = Input.GetKeyDown(keyBind.Key);
                            break;
                        case Define.InputKeyType.Hold:
                            pressed = Input.GetKey(keyBind.Key);
                            break;
                    }

                    if (pressed)
                    {
                        KeyAction.Invoke(keyMap.Key);
                    }
                }
            }
        }

        if (MouseAction != null)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time - _pressedTime >= CLICK_DELAY)
                {
                    MouseAction.Invoke(Define.MouseEvent.LClick);
                    _pressedTime = Time.time;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                MouseAction.Invoke(Define.MouseEvent.RClick);
            }
        }
    }
}
