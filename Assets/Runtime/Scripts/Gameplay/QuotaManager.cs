using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class QuotaManager : MonoBehaviour
{
    [SerializeField] private IntEventChannelSO orderFulfilledChannel;
    [SerializeField] private TextMeshProUGUI quotaText;
    [SerializeField] private Material fireShader;
    [SerializeField] private float normalFlameThreshold = 0.3f; // Percentage of target earnings for normal flame
    [SerializeField] private float extraHotFlameThreshold = 0.6f; // Percentage of target earnings for extra hot flame

    private int _targetEarnings = 1000;
    private int _currentEarnings;
    private float _earningsRate;
    private static readonly int FlameStrength = Shader.PropertyToID("_FlameStrength");

    private void Awake()
    {
        orderFulfilledChannel.OnEventRaised += OrderFulfilled;
    }

    private void Update()
    {
        UpdateFlameEffect();
    }

    private void OrderFulfilled(int value)
    {
        // Calculate earnings based on the value of the order
        int earningsFromOrder = value * 50; // Assuming each ingredient value contributes $50 to the earnings
        _currentEarnings += earningsFromOrder;

        // Update the UI
        UpdateQuotaDisplay();

        // Update flame effect
        UpdateFlameEffect();
    }

    private void UpdateQuotaDisplay()
    {
        quotaText.text = $"{_currentEarnings}";
    }

    private void UpdateFlameEffect()
    {
        float earningsRate = (float)_currentEarnings / _targetEarnings;
        float flameIntensity;

        if (earningsRate >= extraHotFlameThreshold)
        {
            // Calculate the interpolation factor above the extra hot threshold
            flameIntensity = 1 + (earningsRate - extraHotFlameThreshold) / (1 - extraHotFlameThreshold);
            flameIntensity = Mathf.Clamp(flameIntensity, 1, 2); // Clamp between 1 and 2 to prevent exceeding extra hot intensity
        }
        else if (earningsRate >= normalFlameThreshold)
        {
            // Calculate the interpolation factor between normal and extra hot thresholds
            flameIntensity = 1 + (earningsRate - normalFlameThreshold) / (extraHotFlameThreshold - normalFlameThreshold);
            flameIntensity = Mathf.Clamp(flameIntensity, 1, 2); // Clamp to ensure it does not go below normal flame intensity (1)
        }
        else
        {
            // Calculate the interpolation factor below the normal threshold
            flameIntensity = earningsRate / normalFlameThreshold;
            flameIntensity = Mathf.Clamp(flameIntensity, 0, 1); // Clamp to ensure it does not exceed normal flame intensity (1)
        }

        fireShader.SetFloat(FlameStrength, flameIntensity);
    }
} 
