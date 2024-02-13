using UnityEngine;
using UnityEngine.UI;

public class CoffeePlant : Workstation, IProduceItem {
    [SerializeField] private float maxHealth = 10;
    [SerializeField] private GameObject coffeeBeansPrefab;
    [SerializeField] private float regenerationRate = 5f; // CurrentHealth per minute
    [SerializeField] private Image healthSlider;
    private float _currentHealth;
    
    private void Start() {
        _currentHealth = maxHealth;
        UpdateHealthSlider();
    }

    private void Update() {
        RegenerateHealth();
        UpdateHealthSlider();
    }
    
    private void UpdateHealthSlider() {
        healthSlider.fillAmount = _currentHealth / maxHealth;
    }

    private void RegenerateHealth() {
        if (_currentHealth < maxHealth) {
            _currentHealth += Mathf.CeilToInt(regenerationRate * Time.deltaTime / 60f);
            if (_currentHealth > maxHealth) _currentHealth = maxHealth;
        }
    }

    public GameObject ProduceItem() {
        if (_currentHealth <= 0) return null;
        _currentHealth--;
        UpdateHealthSlider();
        return Instantiate(coffeeBeansPrefab, transform.position + Vector3.up, Quaternion.identity);
    }
}