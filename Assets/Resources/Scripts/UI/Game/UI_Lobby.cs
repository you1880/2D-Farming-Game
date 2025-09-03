using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Lobby : UI_Base
{
    private enum Buttons
    {
        StartButton,
        LoadButton,
        QuitButton,
        SettingButton
    }

    private enum GameObjects
    { 
        SelectObject
    }

    private int[] _buttonPositions = { -75, -160, -245 };
    private RectTransform _selectObject;
    private int _currentButton = 0;

    private void OnStartButtonClicked(PointerEventData data)
    {
        UI_SaveLoad ui = Managers.UI.ShowUI<UI_SaveLoad>();
        ui.SetMode(0);
    }

    private void OnLoadButtonClicked(PointerEventData data)
    {
        UI_SaveLoad ui = Managers.UI.ShowUI<UI_SaveLoad>();
        ui.SetMode(1);
    }

    private void OnQuitButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowMessageBoxUI(MessageID.GameQuitMsg, () =>
        {
            Application.Quit();
        });
    }

    private void OnSettingButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowUI<UI_Setting>();
    }

    private void OnButtonEntered(PointerEventData data)
    {
        if (!Enum.TryParse(data.pointerEnter.name, out Buttons button))
        {
            return;
        }

        int enteredButton = (int)button;

        if (_currentButton == enteredButton)
        {
            return;
        }

        _currentButton = enteredButton;
        _selectObject.anchoredPosition = new Vector3(
            _selectObject.anchoredPosition.x,
            _buttonPositions[enteredButton],
            0.0f
        );
    }

    private void BindUIElements()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
    }

    private void GetUIElements()
    {
        _selectObject = Get<GameObject>((int)GameObjects.SelectObject).GetComponent<RectTransform>();
    }

    private void BindButtonEvent()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject btn = GetButton(i).gameObject;

            btn.BindEvent(OnButtonEntered, Define.UIEvent.PointerEnter);

            switch (i)
            {
                case 0:
                    btn.BindEvent(OnStartButtonClicked);
                    break;
                case 1:
                    btn.BindEvent(OnLoadButtonClicked);
                    break;
                case 2:
                    btn.BindEvent(OnQuitButtonClicked);
                    break;
            }
        }

        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(OnSettingButtonClicked);
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
    }
}
