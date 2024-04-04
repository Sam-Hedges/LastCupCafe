using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Kettle : Workstation, IMinigame {
    //Scrollbar UI object
    [SerializeField] private LiquidUI temperatureGauge;
    [SerializeField] private Image progressMeter;
    [SerializeField] private Color filledCol;
    [SerializeField] private Color warningCol;
    [SerializeField] private Color emptyCol;
    [SerializeField] private float startTime = 45.0f;

    // Timer functions
    private float _currentTime = 0.0f;
    private bool _isActive;
    private bool _isOnCooldown;
    private Mug _lastFilledMug;
    private Coroutine _cooldownRoutine;

    private void Start() {
        progressMeter.color = filledCol;
    }

    private void FixedUpdate() {
        if (_isOnCooldown) return;
        if (!_isActive) return;

        _currentTime -= Time.fixedDeltaTime;
        float progress = _currentTime / startTime;
        temperatureGauge.targetFillAmount = progress;

        if (progress < 0.7f) {
            temperatureGauge.targetForegroundColor = filledCol;
        }
        else if (progress < 0.5f) {
            temperatureGauge.targetForegroundColor = warningCol;
        }
        else if (progress <  0.3f) {
            temperatureGauge.targetForegroundColor = emptyCol;
        }
        else if (progress <= 0) {
            _currentTime = 0;
            _cooldownRoutine = StartCoroutine(KettleCooldown(10));
            return;
        }

        FillMug();
    }

    private void FillMug() {
        if (currentlyStoredItem &&
            currentlyStoredItem.TryGetComponent(out Mug mug) != _lastFilledMug) {
            progressMeter.fillAmount += 0.01f;
            progressMeter.fillAmount = Mathf.Clamp(progressMeter.fillAmount, 0, 0.25f);
            if (progressMeter.fillAmount == 0.25f) {
                mug.AddIngredient(IngredientType.Water);
                progressMeter.fillAmount = 0;
                _lastFilledMug = mug;
            }
        }
    }

    private IEnumerator KettleCooldown(float seconds) {
        _isOnCooldown = true;
        float counter = 0;

        progressMeter.fillAmount = 0.25f;
        progressMeter.color = emptyCol;

        while (true) {
            if (counter >= seconds) break;
            yield return new WaitForSeconds(1);
            counter++;

            float progressDelta = -Mathf.Clamp(counter / seconds * 0.25f, 0, 0.25f);
            progressMeter.fillAmount = progressDelta;
        }

        _isOnCooldown = false;
        _cooldownRoutine = null;
        progressMeter.color = filledCol;
        progressMeter.fillAmount = 0f;
        yield return null;
    }

    public override void MinigameButton() {
        if (_isOnCooldown) {
            // TODO: Play can't do SFX
            
        }
        else {
            _currentTime = startTime;
            _isActive = true;
            Debug.Log("Kettle Active");
        }
    }
}