using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [Tooltip("Time in minutes")]
    public float timeRemaining;

    bool timerRunning;

    public bool fail = false;

    public bool mainTimer = false;

    public TextMeshProUGUI timeText;

    public GameObject ui;

    public float timer;
    void Start()
    {
        timer = timeRemaining * 60; //converts time to seconds

        timerRunning = true;
    }

    void Update()
    {
        if (timerRunning)
        {
            if(timer > 0)
            {
                if (mainTimer)
                {
                    timer = ui.GetComponent<Serve>().newTime;
                }
                timer -= Time.deltaTime;
                DisplayTime(timer);
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;
                fail = true;
            }
        }
    }

    //dislays time remaining in a minutes:seconds format
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
