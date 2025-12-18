using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season gameSeason = Season.春天;
    private int monthInSeason;
    public bool gameClockPause;
    private float tikTime;

    private float timeDifference;
    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);
    protected override void Awake()
    {
        base.Awake();
        NewGameTime();
    }
    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    

    private void Start()
    {
        EventHandler.CallGameDataEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
    }
    private void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;
            if(tikTime >= Settings.secondThreshold)
            {
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
        {
            for (int i = 0; i < 60 * 10; i++)
            {
                UpdateGameTime();
            }
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            for(int i = 0; i < 60; i++)
            {
                UpdateGameTime();
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            gameDay++;
            EventHandler.CallGameDayEvent(gameDay, gameSeason);
            EventHandler.CallGameDataEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        }
    }
    private void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true;
    }
    private void OnAfterSceneLoadedEvent()
    {
        gameClockPause = false;
    }
    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 2022;
        gameSeason = Season.春天;

    }
    private void UpdateGameTime()
    {
        gameSecond++;
        if(gameSecond > Settings.secondHold)
        {
            gameMinute++;
            gameSecond = 0;
            if(gameMinute > Settings.minuteHold)
            {
                gameHour++;
                gameMinute = 0;
                if(gameHour > Settings.hourHold)
                {
                    gameDay++;
                    gameHour = 0;
                    if(gameDay > Settings.dayHold)
                    {
                        gameDay = 1;
                        gameMonth++;
                        if(gameMonth > 12)
                        {
                            gameMonth = 1;
                        }
                        monthInSeason--;
                        if(monthInSeason == 0)
                        {
                            monthInSeason = 3;
                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;
                            if(seasonNumber > Settings.seasonHold)
                            {
                                seasonNumber = 0;
                                gameYear++;
                            }
                            gameSeason = (Season)seasonNumber;
                            if(gameYear > 9999)
                            {
                                gameYear = 2022;
                            }
                        }
                    }
                    //刷新地图
                    EventHandler.CallGameDayEvent(gameDay, gameSeason);
                }
                EventHandler.CallGameDataEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            }
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);

            //切换灯光
            EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
        }

    }

    private LightShift GetCurrentLightShift()
    {
        if(GameTime >= Settings.morningTime && GameTime < Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;
            return LightShift.Morning;
        }
        if(GameTime < Settings.morningTime || GameTime >= Settings.nightTime)
        {
            timeDifference = Mathf.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);
            Debug.Log(timeDifference);
            return LightShift.Night;
        }
        return LightShift.Morning;
    }

}
