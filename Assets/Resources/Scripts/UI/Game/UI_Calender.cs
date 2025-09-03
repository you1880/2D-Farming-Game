using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Calender : UI_Base
{
    private enum Buttons { }
    private enum Texts
    {
        TimeText,
        DayText
    }

    private enum Gameobjects { }
    private enum Images
    {
        TimeImage,
        SeasonImage
    }

    private const int AFTERNOON_TIME = 12;
    private GameTimeManager timeManager => Managers.Time;
    private TextMeshProUGUI _timeText;
    private TextMeshProUGUI _dayText;
    [SerializeField] private Sprite _morningIcon;
    [SerializeField] private Sprite _noonIcon;
    [SerializeField] private Sprite _eveningIcon;
    [SerializeField] private Sprite _nightIcon;
    [SerializeField] private Sprite _springIcon;
    [SerializeField] private Sprite _summerIcon;
    [SerializeField] private Sprite _fallIcon;
    [SerializeField] private Sprite _winterIcon;
    private Image _timeImage;
    private Image _seasonImage;

    private void SetTimeImage()
    {
        Sprite sprite = null;
        Define.TimeSection timeSection = timeManager.GetTimeSection();
        switch (timeSection)
        {
            case Define.TimeSection.Morning:
                sprite = _morningIcon;
                break;
            case Define.TimeSection.Afternoon:
                sprite = _noonIcon;
                break;
            case Define.TimeSection.Evening:
                sprite = _eveningIcon;
                break;
            case Define.TimeSection.Night:
                sprite = _nightIcon;
                break;
        }

        if (sprite != null)
        {
            _timeImage.sprite = sprite;
        }
    }

    private void SetSeasonImage()
    {
        Sprite sprite = null;
        Define.Season season = timeManager.CurrentSeason;
        switch (season)
        {
            case Define.Season.Spring:
                sprite = _springIcon;
                break;
            case Define.Season.Summer:
                sprite = _summerIcon;
                break;
            case Define.Season.Fall:
                sprite = _fallIcon;
                break;
            case Define.Season.Winter:
                sprite = _winterIcon;
                break;
        }

        if (sprite != null)
        {
            _seasonImage.sprite = sprite;
        }
    }

    private void SetCalenderText()
    {
        int hour = timeManager.CurrentHour;
        int min = timeManager.CurrentMinute;
        int day = timeManager.CurrentDay;
        string weekName = timeManager.CurrentWeekName;

        if (hour >= 0 && hour <= 2)
        {
            _timeText.color = Color.red;
        }
        else
        {
            _timeText.color = Color.white;
        }

        string sep = hour >= AFTERNOON_TIME ? "오후" : "오전";
        int displayHour = hour % AFTERNOON_TIME == 0 ? AFTERNOON_TIME : hour % AFTERNOON_TIME;

        _timeText.text = $"{sep} {displayHour:D2} : {min:D2}";
        _dayText.text = $"{weekName}, {day}";
    }

    private void SetCalender()
    {
        SetTimeImage();
        SetSeasonImage();
        SetCalenderText();
    }

    private void BindUIElements()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
    }

    private void GetUIElements()
    {
        _timeText = GetText((int)Texts.TimeText);
        _dayText = GetText((int)Texts.DayText);
        _timeImage = GetImage((int)Images.TimeImage);
        _seasonImage = GetImage((int)Images.SeasonImage);
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        SetCalender();
    }

    private void OnEnable()
    {
        timeManager.OnTimeChanged -= SetCalender;
        timeManager.OnTimeChanged += SetCalender;
    }

    private void OnDisable()
    {
        timeManager.OnTimeChanged -= SetCalender;
    }
}
