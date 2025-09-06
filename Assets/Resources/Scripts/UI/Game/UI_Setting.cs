using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Setting : UI_Base
{
    public enum Buttons
    {
        GraphicButton,
        SoundButton,
        GameButton,
        ExitButton,
        LobbyButton,
        QuitButton
    }

    private enum Texts
    {
        BgmValueText,
        EffectValueText
    }

    public enum GameObjects
    {
        GraphicSettings,
        SoundSettings,
        GameSettings,
        WindowedToggle,
        ResolutionDropdown,
        BgmSlider,
        EffectSlider
    }

    private enum SettingSlot { Graphic, Sound, Game };
    private List<Resolution> _resolutions;
    private GameObject _graphicSetting;
    private GameObject _soundSetting;
    private GameObject _gameSetting;
    private Toggle _windowedToggle;
    private TMP_Dropdown _resolutionDropdown;
    private Slider _bgmSlider;
    private Slider _effectSlider;
    private TextMeshProUGUI _bgmValue;
    private TextMeshProUGUI _effectValue;
    private SettingSlot _currentSlot = SettingSlot.Graphic;

    private void OnSettingButtonClicked(PointerEventData data)
    {
        string suffix = "Button";
        string btnName = data.pointerClick.name;
        if (!btnName.EndsWith(suffix))
        {
            return;
        }

        btnName = btnName[..^suffix.Length];

        if (!Enum.TryParse(btnName, out SettingSlot select))
        {
            return;
        }
        

        if (_currentSlot == select)
        {
            return;
        }

        _currentSlot = select;
        InitializeMainMenu();
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        Managers.UI.CloseUI(this.gameObject);
    }

    private void OnLobbyButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowMessageBoxUI(MessageID.NoSaveWarning, () => { Managers.Scene.LoadNextScene(Define.SceneType.Lobby); });
    }

    private void OnQuitButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowMessageBoxUI(MessageID.NoSaveWarning, () => { Application.Quit(); });
    }

    #region Toggle
    private void OnToggleChanged(bool isFullScreen)
    {
        Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.fullScreen = isFullScreen;
    }

    private void InitScreenToggle()
    {
        if(_windowedToggle == null)
        {
            return;
        }
        
        _windowedToggle.isOn = Screen.fullScreen;
        _windowedToggle.onValueChanged.AddListener(OnToggleChanged);
    }
    #endregion

    #region Slider
    private void OnBgmSliderChanged(float sliderValue)
    {
        Managers.Sound.SetBgmVolume(sliderValue);
        _bgmValue.text = $"{sliderValue}";
    }

    private void OnEffectSliderChanged(float sliderValue)
    {
        Managers.Sound.EffectVolume = sliderValue;
        _effectValue.text = $"{sliderValue}";
    }

    private void InitSlider()
    {
        _bgmSlider.wholeNumbers = _effectSlider.wholeNumbers = true;
        _bgmSlider.minValue = _effectSlider.minValue = 0;
        _bgmSlider.maxValue = _effectSlider.maxValue = 100;

        _bgmSlider.value = Managers.Sound.BgmVolume * 100.0f;
        _effectSlider.value = Managers.Sound.EffectVolume * 100.0f;

        _bgmValue.text = $"{_bgmSlider.value}";
        _effectValue.text = $"{_effectSlider.value}";

        _bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        _effectSlider.onValueChanged.AddListener(OnEffectSliderChanged);
    }
    #endregion

    #region Dropdown
    private void OnResolutionChanged(int index)
    {
        Resolution res = _resolutions[index];
        bool isFullScreen = Screen.fullScreen;

        Screen.SetResolution(res.width, res.height, isFullScreen);
    }

    private void InitDropdown()
    {
        _resolutions = new List<Resolution>(Screen.resolutions);
        _resolutionDropdown.ClearOptions();

        int currentIndex = 0;
        List<string> options = new List<string>();

        for (int i = 0; i < _resolutions.Count; i++)
        {
            Resolution res = _resolutions[i];
            string option = $"{res.width} x {res.height} @ {res.refreshRate}Hz";

            options.Add(option);

            if (res.width == Screen.width &&
                res.height == Screen.height
                && res.refreshRate == Screen.currentResolution.refreshRate)
            {
                currentIndex = i;
            }
        }

        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = currentIndex;
        _resolutionDropdown.RefreshShownValue();

        _resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }
    #endregion
    
    private void BindUIElements()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
    }

    private void GetUIElements()
    {
        _graphicSetting = GetObject((int)GameObjects.GraphicSettings);
        _soundSetting = GetObject((int)GameObjects.SoundSettings);
        _gameSetting = GetObject((int)GameObjects.GameSettings);
        _windowedToggle = GetObject((int)GameObjects.WindowedToggle)?.GetComponent<Toggle>();
        _resolutionDropdown = GetObject((int)GameObjects.ResolutionDropdown)?.GetComponent<TMP_Dropdown>();
        _bgmSlider = GetObject((int)GameObjects.BgmSlider)?.GetComponent<Slider>();
        _effectSlider = GetObject((int)GameObjects.EffectSlider)?.GetComponent<Slider>();
        _bgmValue = GetText((int)Texts.BgmValueText);
        _effectValue = GetText((int)Texts.EffectValueText);
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
        GetButton((int)Buttons.LobbyButton).gameObject.BindEvent(OnLobbyButtonClicked);
        GetButton((int)Buttons.QuitButton).gameObject.BindEvent(OnQuitButtonClicked);
        GetButton((int)Buttons.GraphicButton).gameObject.BindEvent(OnSettingButtonClicked);
        GetButton((int)Buttons.GameButton).gameObject.BindEvent(OnSettingButtonClicked);
        GetButton((int)Buttons.SoundButton).gameObject.BindEvent(OnSettingButtonClicked);
    }

    private void InitializeMainMenu()
    {
        switch(_currentSlot)
        {
            case SettingSlot.Game:
                _gameSetting.SetActive(true);
                _graphicSetting.SetActive(false);
                _soundSetting.SetActive(false);
                break;
            case SettingSlot.Graphic:
                _gameSetting.SetActive(false);
                _graphicSetting.SetActive(true);
                _soundSetting.SetActive(false);
                break;
            case SettingSlot.Sound:
                _gameSetting.SetActive(false);
                _graphicSetting.SetActive(false);
                _soundSetting.SetActive(true);
                break;
            default:
                return;
        }
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        InitSlider();
        InitScreenToggle();
        InitDropdown();
        InitializeMainMenu();
    }
}
