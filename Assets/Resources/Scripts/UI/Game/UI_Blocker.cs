using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Blocker : UI_Base
{
    private enum Texts
    {
        BlockerText
    }

    private TextMeshProUGUI _blockerText;

    public void SetText(string text)
    {
        if (_blockerText == null)
        {
            return;
        }

        _blockerText.text = text;
    }

    private void BindUIElements()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    private void GetUIElements()
    {
        _blockerText = GetText((int)Texts.BlockerText);
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
    }

    private void OnEnable()
    {
        gameObject.transform.SetAsLastSibling();
    }
}
