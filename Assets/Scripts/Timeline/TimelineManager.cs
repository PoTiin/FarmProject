using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : Singleton<TimelineManager>
{
    public PlayableDirector startDirector;
    private PlayableDirector currentDirector;

    private bool isDone;
    public bool IsDone
    {
        set
        {
            isDone = value;
        }
    }
    private bool isPause;

    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
    }
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        startDirector.played += OnPlayed;
        startDirector.stopped += OnStopped;
    }

    

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

   

    public void PauseTimeline(PlayableDirector director)
    {
        currentDirector = director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        isPause = true;
    }
    private void Update()
    {
        if(isPause && Input.GetKeyDown(KeyCode.Space) && isDone)
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
        //if (startDirector.playableGraph.IsValid() && startDirector.playableGraph.IsPlaying())
        //{
        //    //EventHandler.CallUpdateGameStateEvent(GameState.Pause);
        //}
    }
    private void OnAfterSceneLoadedEvent()
    {
        if (!startDirector.isActiveAndEnabled)
            EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
    }
    private void OnStartNewGameEvent(int obj)
    {
        if (startDirector != null)
            startDirector.Play();
    }
    private void OnStopped(PlayableDirector director)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
        startDirector.enabled = false;
    }

    private void OnPlayed(PlayableDirector director)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);
    }
}
