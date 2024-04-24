using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public Image timeBar;
    public bool runAtStart = false;
    public float timerDurationMinutes = 1; // Default timer duration in minutes
    public UnityAction OnTimerCompleteAction;
    public UnityEvent OnTimerCompleteEvent;

    private float _totalTimeInSeconds;
    private bool _isTimerRunning;

    private void Start()
    {
        InitTimer(timerDurationMinutes);

        if (runAtStart)
        {
            StartTimer();
        }
    }

    private void InitTimer(float minutes)
    {
        timerDurationMinutes = minutes;
        _totalTimeInSeconds = timerDurationMinutes * 60; // Converts time to seconds
        UpdateTimeDisplay(_totalTimeInSeconds);
    }

    public void StartTimer()
    {
        if (!_isTimerRunning)
        {
            _isTimerRunning = true;
        }
    }

    private void Update()
    {
        if (_isTimerRunning)
        {
            _totalTimeInSeconds -= Time.deltaTime;
            UpdateTimeDisplay(_totalTimeInSeconds);

            if (_totalTimeInSeconds <= 0)
            {
                FinishTimer();
            }
        }
    }

    private void FinishTimer()
    {
        _isTimerRunning = false;
        _totalTimeInSeconds = 0;
        UpdateTimeDisplay(_totalTimeInSeconds);
        OnTimerCompleteAction?.Invoke(); // Triggers the event
        OnTimerCompleteEvent?.Invoke();
    }

    // Displays time remaining in a minutes:seconds format and updates the time bar
    private void UpdateTimeDisplay(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        if (timeText) timeText.text = $"{minutes:0}:{seconds:00}";
        if (timeBar) timeBar.fillAmount = timeToDisplay / (timerDurationMinutes * 60);
    }
}