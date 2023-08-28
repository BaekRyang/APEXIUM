using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayTimer : MonoBehaviour
{
    public static float playTime;

    public TMP_Text timerTextHm;
    public TMP_Text timerTextMs;

    private void Awake()
    {
        timerTextHm ??= GameObject.Find("TimerTextHM").GetComponent<TMP_Text>();
        timerTextMs ??= GameObject.Find("TimerTextMS").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        playTime += Time.deltaTime;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        timerTextHm.text = $"{((int)playTime / 60):00}:{(int)playTime % 60:00}";
        timerTextMs.text = $"{(int)(playTime * 100)                   % 100:00}";
    }

    public static void ResetTimer() => playTime = 0;
}