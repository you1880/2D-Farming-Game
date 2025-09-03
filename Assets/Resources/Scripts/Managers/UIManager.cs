using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private Dictionary<string, UI_Base> _uiDict = new Dictionary<string, UI_Base>();
    private GameObject _openedPauseUI = null;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");

            if (root == null)
            {
                root = new GameObject { name = "@UI_Root" };
                Canvas canvas = root.GetOrAddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
            }

            return root;
        }
    }

    public void Init()
    {
        Managers.Input.KeyAction -= OnKeyboardEvent;
        Managers.Input.KeyAction += OnKeyboardEvent;
    }

    public void OnKeyboardEvent(Define.KeyBoardEvent keyBoardEvent)
    {
        switch (keyBoardEvent)
        {
            case Define.KeyBoardEvent.Inventory:
                ShowPauseUI<UI_Inventory>();
                break;
            case Define.KeyBoardEvent.Setting:
                ShowPauseUI<UI_Setting>();
                break;
        }
    }

    public UI_MessageBox ShowMessageBoxUI(MessageID messageID, Action onCompleted = null, bool isConfirmMode = false)
    {
        UI_MessageBox messageBox = ShowUI<UI_MessageBox>();
        string message = MessageTable.GetMessage(messageID);

        messageBox.SetMessageBox(message, onCompleted, isConfirmMode);
        Managers.Time.StopGame();

        return messageBox;
    }

    public T ShowUI<T>(string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        UI_Base ui;

        if (_uiDict.TryGetValue(name, out ui))
        {
            if (ui == null)
            {
                _uiDict.Remove(name);
            }
            else
            {
                if (!ui.gameObject.activeSelf)
                {
                    ui.gameObject.SetActive(true);
                    ui.Refresh();
                }

                return (T)ui;
            }
        }

        GameObject obj = Managers.Resource.Instantiate($"UI/{name}");

        if (obj == null)
        {
            return null;
        }

        ui = obj.GetOrAddComponent<T>();
        obj.transform.SetParent(Root.transform, false);
        _uiDict[name] = ui;

        return ui as T;
    }

    public T ShowPauseUI<T>() where T : UI_Base
    {
        if (!CanShowPauseUI())
        {
            return null;
        }

        if (_openedPauseUI != null)
        {
            CloseUI(_openedPauseUI);
            _openedPauseUI = null;

            return null;
        }

        T ui = ShowUI<T>();

        if (ui != null)
        {
            Managers.Time.StopGame();
        }

        _openedPauseUI = ui.gameObject;

        return ui;
    }

    public void CloseUI(GameObject ui)
    {
        if (ui == null)
        {
            return;
        }

        Managers.Time.ResumeGame();
        ui.SetActive(false);
    }

    public void ClearUI(GameObject ui)
    {
        if (ui == null)
        {
            return;
        }

        if (_uiDict.Remove(ui.name))
        {
            Managers.Resource.Destroy(ui);
            ui = null;
        }

        Managers.Time.ResumeGame();
    }

    public void ClearUIDictionary()
    {
        _uiDict.Clear();
    }
    
    private bool CanShowPauseUI()
    {
        Define.SceneType currentSceneType = Managers.Scene.CurrentScene?.CurrentScene ?? Define.SceneType.Unknown;

        return currentSceneType == Define.SceneType.Main || currentSceneType == Define.SceneType.Cave;
    }
}
