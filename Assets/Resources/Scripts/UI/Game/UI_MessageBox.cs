using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MessageBox : UI_Base
{
    private enum Texts
    {
        MessageBoxText,
    }

    private enum Buttons
    {
        OkButton,
        NoButton
    }

    private TextMeshProUGUI _msgBoxText;
    private Action _callback;
    private string _text;
    private bool _isConfirmMode;

    public void SetMessageBox(string text, Action onCompleted = null, bool isConfirmMode = false)
    {
        _text = text;
        _callback = onCompleted;
        _isConfirmMode = isConfirmMode;

        if (_msgBoxText != null)
        {
            _msgBoxText.text = _text;
        }
    }

    private void OnOkButtonClicked(PointerEventData data)
    {
        _callback?.Invoke();
        Managers.UI.CloseUI(this.gameObject);
    }

    private void OnNoButtonClicked(PointerEventData data)
    {
        Managers.UI.CloseUI(this.gameObject);
    }

    private void BindUIElements()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.OkButton).gameObject.BindEvent(OnOkButtonClicked);
        Button noButton = GetButton((int)Buttons.NoButton);

        if (!_isConfirmMode)
        {
            noButton.gameObject.BindEvent(OnNoButtonClicked);
        }
        else
        {
            noButton.gameObject.SetActive(false);
        }
    }

    private void GetUIElements()
    {
        _msgBoxText = GetText((int)Texts.MessageBoxText);
        _msgBoxText.text = _text;
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
    }

    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    private void OnDisable()
    {
        _callback = null;
        _text = "";
    }
}
