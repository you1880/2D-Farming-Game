using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameTimeManager
{
    private const float UPDATE_INTERVAL = 5.0f;
    private const int SEASONS_PER_YEAR = 4;
    private const int DAYS_PER_SEASON = 28;
    private const int HOURS_PER_DAY = 24;
    private const int MINUTES_PER_HOUR = 60;
    private const int FORCE_SLEEP_TIME = 2;
    private const int MORNING_TIME = 6;
    private const int AFTERNOON_TIME = 12;
    private const int EVENING_TIME = 18;
    private const int NIGHT_TIME = 21;
    private readonly string[] seasonNames = { "봄", "여름", "가을", "겨울" };
    private readonly string[] weekNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
    private Color _dayColor = Color.white;
    private Color _nightColor = new Color(0.1f, 0.1f, 0.25f);
    private Light2D _globalLight;
    private Light2D GlobalLight
    {
        get
        {
            if (_globalLight != null)
            {
                return _globalLight;
            }

            GameObject obj = GameObject.FindWithTag("GlobalLight");
            if (obj != null)
            {
                _globalLight = obj.GetComponent<Light2D>();

                if (_globalLight != null)
                {
                    return _globalLight;
                }
            }
            return null;
        }
    }
    private bool _isInitialized = false;
    private bool _isEndDay = false;
    private Coroutine _timeFlowCoroutine = null;
    public string CurrentSeasonName { get { return seasonNames.SafeGetArrayValue((int)CurrentSeason, ""); } }
    public string CurrentWeekName { get { return weekNames.SafeGetArrayValue(CurrentDay % 7, ""); } }
    public int CurrentYear { get; private set; } = 1;
    public Define.Season CurrentSeason { get; private set; } = Define.Season.Spring;
    public int CurrentDay { get; private set; } = 1;
    public int CurrentHour { get; private set; } = MORNING_TIME;
    public int CurrentMinute { get; private set; } = 0;
    public bool StartDay = true;
    public event Action OnTimeChanged;

    public void Init()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;

            InitDate();
        }
    }

    public void ResumeGame()
    {
        if (Time.timeScale == 0.0f)
        {
            Time.timeScale = 1.0f;
        }
    }

    public void StopGame()
    {
        if (Time.timeScale == 1.0f)
        {
            Time.timeScale = 0.0f;
        }
    }

    public void FlowTime()
    {
        if (_isEndDay)
        {
            _isEndDay = false;
        }
        
        if (_timeFlowCoroutine != null)
        {
            Managers.TerminateCoroutine(_timeFlowCoroutine);
            _timeFlowCoroutine = null;
        }

        _timeFlowCoroutine = Managers.RunCoroutine(UpdateTime());
    }

    public void StopTimeFlow()
    {
        if (_timeFlowCoroutine != null)
        {
            Managers.TerminateCoroutine(_timeFlowCoroutine);
            _timeFlowCoroutine = null;
        }
    }

    public int GetCurrentTimeInMinutes()
    {
        return (((CurrentYear * SEASONS_PER_YEAR + (int)CurrentSeason) * DAYS_PER_SEASON + (CurrentDay - 1)) * HOURS_PER_DAY * MINUTES_PER_HOUR) 
                + (CurrentHour * 60) 
                + CurrentMinute;
    }

    public Define.TimeSection GetTimeSection()
    {
        if (CurrentHour >= MORNING_TIME && CurrentHour < AFTERNOON_TIME)
        {
            return Define.TimeSection.Morning;
        }
        else if (CurrentHour >= AFTERNOON_TIME && CurrentHour < EVENING_TIME)
        {
            return Define.TimeSection.Afternoon;
        }
        else if (CurrentHour >= EVENING_TIME && CurrentHour < NIGHT_TIME)
        {
            return Define.TimeSection.Evening;
        }
        else if (CurrentHour >= NIGHT_TIME || CurrentHour <= FORCE_SLEEP_TIME)
        {
            return Define.TimeSection.Night;
        }

        return Define.TimeSection.Morning;
    }

    public void SetNextDay()
    {
        if (_timeFlowCoroutine != null)
        {
            Managers.TerminateCoroutine(_timeFlowCoroutine);
            _timeFlowCoroutine = null;
        }

        AdvanceTime();
        CurrentHour = MORNING_TIME;
        CurrentMinute = 0;
        StartDay = true;

        PlayerController playerController = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
        Managers.Scene.LoadNextScene(Define.SceneType.DayEnd, playerController);
    }

    private void AdvanceTime()
    {
        if(CurrentHour >= 0 && CurrentHour <= 2)
        {
            return;
        }

        CurrentDay += 1;

        if(CurrentDay > 28)
        {
            CurrentSeason++;
            CurrentDay = 1;
        }

        if(CurrentSeason > Define.Season.Winter)
        {
            CurrentSeason = Define.Season.Spring;
            CurrentYear++;
        }
    }

    private void InitDate()
    {
        Data.Player.PlayerData playerData = Managers.Data.UserDataManager.CurrentData;

        if (playerData != null)
        {
            CurrentYear = playerData.date.year;
            CurrentSeason = playerData.date.season;
            CurrentDay = playerData.date.day;
        }
    }

    private void CalculateTime(int minute = 10)
    {
        CurrentMinute += minute;

        if (CurrentMinute >= MINUTES_PER_HOUR)
        {
            CurrentHour += CurrentMinute / MINUTES_PER_HOUR;
            CurrentMinute = CurrentMinute % MINUTES_PER_HOUR;
        }

        if (CurrentHour >= HOURS_PER_DAY)
        {
            CurrentDay += CurrentHour / HOURS_PER_DAY;
            CurrentHour = CurrentHour % HOURS_PER_DAY;
        }

        if (CurrentDay > DAYS_PER_SEASON)
        {
            CurrentSeason += (CurrentDay - 1) / DAYS_PER_SEASON;
            CurrentDay = ((CurrentDay - 1) % DAYS_PER_SEASON) + 1;
        }

        if ((int)CurrentSeason == SEASONS_PER_YEAR)
        {
            CurrentYear += (int)CurrentSeason / SEASONS_PER_YEAR;
            CurrentSeason = (Define.Season)((int)CurrentSeason % SEASONS_PER_YEAR);
        }

        OnTimeChanged?.Invoke();
    }

    private void UpdateDayLight()
    {
        if (GlobalLight == null)
        {
            return;
        }

        if (CurrentHour >= EVENING_TIME || CurrentHour < MORNING_TIME)
        {
            if (CurrentHour >= EVENING_TIME && CurrentHour < NIGHT_TIME)
            {
                float t = ((CurrentHour - EVENING_TIME) * 60.0f + CurrentMinute) / 180.0f;
                GlobalLight.color = Color.Lerp(_dayColor, _nightColor, t);
            }
            else
            {
                GlobalLight.color = _nightColor;
            }
        }
        else
        {
            _globalLight.color = _dayColor;
        }
    }
    
    private IEnumerator UpdateTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(UPDATE_INTERVAL);

            CalculateTime();
            UpdateDayLight();

            if (!_isEndDay && CurrentHour == FORCE_SLEEP_TIME && CurrentMinute >= 0)
            {
                _isEndDay = true;
                SetNextDay();

                yield break;
            }
        }
    }
}
