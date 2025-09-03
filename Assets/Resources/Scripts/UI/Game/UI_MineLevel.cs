using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_MineLevel : UI_Base
{
    private enum Texts
    {
        MIneLevelText,
    }

    [SerializeField] private CaveScene _caveScene;
    private TextMeshProUGUI _levelText;

    private void SetLevelText(int level)
    {
        if (_levelText == null)
        {
            return;
        }

        _levelText.text = $"{level} 층";
    }

    private void BindUIElements()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    private void GetUIElements()
    {
        _levelText = GetText((int)Texts.MIneLevelText);
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
    }

    private void OnEnable()
    {
        if (_caveScene == null)
        {
            BaseScene baseScene = Managers.Scene.CurrentScene;

            if (baseScene is CaveScene caveScene)
            {
                _caveScene = caveScene;
            }
        }

        if (_caveScene == null)
        {
            return;
        }

        _caveScene.OnLevelChanged -= SetLevelText;
        _caveScene.OnLevelChanged += SetLevelText;
    }

    private void OnDisable()
    {
        if (_caveScene == null)
        {
            return;
        }
        
        _caveScene.OnLevelChanged -= SetLevelText;
    }
}
