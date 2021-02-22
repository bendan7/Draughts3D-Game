using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameUI : MonoBehaviour
{

    public TMPro.TextMeshProUGUI clock;
    public TMPro.TextMeshProUGUI player;
    private Stopwatch _stopwatch;
    private TimeSpan _elapsed;


    public void StartNewGame(PlayerColor firstPlayer)
    {
        ClockStart();
        UpdateActivePlayer(firstPlayer);
    }

    public void UpdateActivePlayer(PlayerColor activePlayer)
    {
        player.text = activePlayer.ToString();
    }


    public void GameEnd()
    {

    }

    private void ClockStart()
    {
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    private void ClockStop()
    {
        _stopwatch.Stop();
    }

    private void ClockReset()
    {
        _stopwatch.Reset();
    }

    private void UpdateStopwatchText(int seconds)
    {
        clock.text = seconds.ToString();
    }

    void FixedUpdate()
    {
        if (_elapsed.Seconds < _stopwatch.Elapsed.Seconds)
        {
            UpdateStopwatchText(_stopwatch.Elapsed.Seconds);
        }

        _elapsed = _stopwatch.Elapsed;   
    }

}
