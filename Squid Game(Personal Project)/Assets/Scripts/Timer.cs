using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 2;
    public bool timerIsRunning = false;
    public event Action onTimeOver;

    private void Start()
    {
        timerIsRunning = true;
        GameManager.instance.onGameOver += EndGame;
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                if (onTimeOver != null)
                {
                    onTimeOver();
                }
            }
        }

        UpdateUI(timeRemaining);
    }

    private void UpdateUI(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        UIManager.instance.UpdateTimerText(minutes, seconds);
    }

    private void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
    }

    private void EndGame()
    {
        timerIsRunning = false;
    }
}
