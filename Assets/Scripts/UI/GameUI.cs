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
    public TMPro.TextMeshProUGUI WiningText;
    public GameObject ExitButton;

    private Stopwatch _stopwatch;
    private bool _isStopwatchActive = false;
    private TimeSpan _displayTime  = TimeSpan.Zero;

    private void OnEnable()
    {
        StopwatchText.gameObject.SetActive(false);
        PlayerText.gameObject.SetActive(false);
        WiningText.gameObject.SetActive(false);
        ExitButton.SetActive(false);
    }

    public void SetActivePlayer(PlayerColor activePlayer)
    {
        PlayerText.text = activePlayer.ToString();
    }

    void ClockStart()
    {
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
        _isStopwatchActive = true;
        UpdateStopwatchText(_stopwatch.Elapsed.Seconds);
    }

    public void SetWinner(PlayerColor playerColor)
    {
        WiningText.text = $"{playerColor}            Wins";
        WiningText.gameObject.SetActive(true);
        _stopwatch.Stop();

        ExitButton.transform.localScale *= 1.5f;
    }

    void UpdateStopwatchText(int seconds)
    {
        StopwatchText.text = seconds.ToString();
    }

    void FixedUpdate()
    {
        if (_isStopwatchActive && _stopwatch !=null)
        {
            if (_displayTime.Seconds < _stopwatch.Elapsed.Seconds)
            {
                UpdateStopwatchText(_stopwatch.Elapsed.Seconds);
            }

            _displayTime = _stopwatch.Elapsed;
        }
 
    }

    internal void GameEnd()
    {
        StopwatchText.gameObject.SetActive(false);
        PlayerText.gameObject.SetActive(false);
        WiningText.gameObject.SetActive(false);
        ExitButton.SetActive(false);
    }

    internal void StartNewGame(PlayerColor activePlayer)
    {
        StopwatchText.gameObject.SetActive(true);
        PlayerText.gameObject.SetActive(true);
        ExitButton.SetActive(true);
        WiningText.gameObject.SetActive(false);
        ClockStart();
    }
}
