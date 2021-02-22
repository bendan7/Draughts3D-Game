using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameUI : MonoBehaviour
{

    public TMPro.TextMeshProUGUI StopwatchText;
    public TMPro.TextMeshProUGUI PlayerText;

    private Stopwatch _stopwatch;
    private bool _isStopwatchActive = false;
    private TimeSpan _displayTime;


    public void SetActivePlayer(PlayerColor activePlayer)
    {
        PlayerText.text = activePlayer.ToString();
    }

    public void ClockStart()
    {
        _stopwatch = new Stopwatch();
        _isStopwatchActive = true;
        _stopwatch.Start();
        UpdateStopwatchText(_stopwatch.Elapsed.Seconds);
    }

    void UpdateStopwatchText(int seconds)
    {
        StopwatchText.text = seconds.ToString();
    }

    void FixedUpdate()
    {
        if (_isStopwatchActive)
        {
            if (_displayTime.Seconds < _stopwatch.Elapsed.Seconds)
            {
                UpdateStopwatchText(_stopwatch.Elapsed.Seconds);
            }

            _displayTime = _stopwatch.Elapsed;
        }
 
    }

}
