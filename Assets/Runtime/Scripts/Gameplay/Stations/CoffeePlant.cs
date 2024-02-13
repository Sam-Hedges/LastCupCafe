using UnityEngine;
using UnityEngine.UI;

public class CoffeePlant : Workstation, IProduceItem {
    [SerializeField] private float maxHealth = 10;
    [SerializeField] private GameObject coffeeBeansPrefab;
    [SerializeField] private float regenerationRate = 5f; // CurrentHealth per minute
    [SerializeField] private Image healthSlider;
    [SerializeField] private Color highHealthColor;
    [SerializeField] private Color mediumHealthColor;
    [SerializeField] private float mediumHealthThreshold = 6;
    [SerializeField] private Color lowHealthColor;
    [SerializeField] private float lowHealthThreshold = 3;
    private float _currentHealth;
    
    private void Start() {
        _currentHealth = maxHealth;
        UpdateHealthSlider();
    }

    private void Update() {
        if (_currentHealth > 0) RegenerateHealth();
        UpdateHealthSlider();
    }
    
    private void UpdateHealthSlider() {
        healthSlider.fillAmount = _currentHealth / maxHealth;
        if (_currentHealth > mediumHealthThreshold) {
            healthSlider.color = highHealthColor;
        } else if (_currentHealth > lowHealthThreshold) {
            healthSlider.color = mediumHealthColor;
        } else {
            healthSlider.color = lowHealthColor;
        }
    }

    private void RegenerateHealth() {
        if (_currentHealth >= maxHealth) return;
        _currentHealth += regenerationRate * Time.deltaTime / 60;
        if (_currentHealth > maxHealth) _currentHealth = maxHealth;
    }

    public GameObject ProduceItem() {
        if (_currentHealth <= 0) return null;
        _currentHealth -= 1;
        UpdateHealthSlider();
        return Instantiate(coffeeBeansPrefab, transform.position + Vector3.up, Quaternion.identity);
    }
}