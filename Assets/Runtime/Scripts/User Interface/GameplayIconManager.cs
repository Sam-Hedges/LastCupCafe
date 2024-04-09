using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayIconManager : MonoBehaviour
{
    [Header("Listening on Channels")] [SerializeField]
    private GameObjectEventChannelSO _gameobjectStateUpdateChannel;

    [Header("Broadcasting on Channels")] [SerializeField]
    private GameObjectEventChannelSO _iconCanvasReadyChannel;

    [Header("Base UI Prefabs")] [SerializeField]
    private GameObject defaultBaseIcon;
    [SerializeField] private GameObject countBaseIcon;
    [SerializeField] private GameObject healthBaseIcon;

    [Header("Workstation UI Prefabs")] [SerializeField]
    private GameObject coffeeMachineUI;

    [Header("Ingredient UI Prefabs")] [SerializeField]
    private GameObject milkIcon;
    [SerializeField] private GameObject coffeeBeansIcon;
    [SerializeField] private GameObject coffeeGroundsIcon;
    [SerializeField] private GameObject chocolateIcon;
    [SerializeField] private GameObject caramelIcon;
    
    [Header("Icon Settings")] [SerializeField]
    private float iconHeight = 1;
    [SerializeField] private float iconScale = 1;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Sprite emptySlotIcon;
    private Dictionary<GameObject, GameObject> _gameplayIcons = new Dictionary<GameObject, GameObject>();

    private void Awake() {
        _gameobjectStateUpdateChannel.OnEventRaised += AddIcon;
        _iconCanvasReadyChannel.RaiseEvent(gameObject);
    }

    private void OnDestroy() {
        _gameobjectStateUpdateChannel.OnEventRaised -= AddIcon;
    }

    private void Update() {
        foreach (var keyValuePair in _gameplayIcons) {
            UpdateIconPosition(keyValuePair.Key);
        }
    }

    private void UpdateIconPosition(GameObject origin) {
        if (_gameplayIcons.TryGetValue(origin, out GameObject icon)) {
            Vector2 screenPoint =
                Camera.main.WorldToScreenPoint(origin.transform.position + new Vector3(0, iconHeight, 0.25f));
            icon.transform.position = screenPoint;
            icon.transform.localScale = Vector3.one * iconScale;
        }
    }

    private void CheckForIcon(GameObject go, GameObject prefab) {
        // Check if the gameobject already has an icon
        if (!_gameplayIcons.TryGetValue(go, out GameObject icon)) {
            // Create icon if not exist
            icon = Instantiate(prefab, transform);
            _gameplayIcons[go] = icon;
        }
    }

    public void AddIcon(GameObject go) {
        if (go.TryGetComponent(out Mug mug)) {
            return;
        }

        if (go.TryGetComponent(out Ingredient ingredient)) {
            switch (ingredient.IngredientType) {
                case IngredientType.Milk:
                    CheckForIcon(go, iconPrefab);
                    UpdateIconPosition(ingredient.gameObject);
                    break;
                case IngredientType.CoffeeBeans:
                    CheckForIcon(go, iconPrefab);
                    UpdateIconPosition(ingredient.gameObject);
                    break;
                case IngredientType.CoffeeGrounds:
                    CheckForIcon(go, iconPrefab);
                    UpdateIconPosition(ingredient.gameObject);
                    break;
                case IngredientType.ChocolatePowder:
                    CheckForIcon(go, iconPrefab);
                    UpdateIconPosition(ingredient.gameObject);
                    break;
                case IngredientType.CaramelSyrup:
                    CheckForIcon(go, iconPrefab);
                    UpdateIconPosition(ingredient.gameObject);
                    break;
                case IngredientType.Water:
                    // Can't hold water directly
                    break;
            }

            return;
        }

        if (go.TryGetComponent(out Workstation workstation)) {
            
            switch (workstation) {
                case Kettle:
                    break;
                case CoffeeMachine:
                    CheckForIcon(go, coffeeMachineUI);
                    UpdateIconPosition(workstation.gameObject);
                    // Disgusting Code
                    (CoffeeMachine)workstation.InitUI(_gameplayIcons[workstation.gameObject].GetComponent<CoffeeMachineMG>());
                    break;
                default:
                    break;
            }

            // Check if the workstation already has an icon
            CheckForIcon(go, iconPrefab);

            // Additional conditions for minigame start, success, failure can be added here

            // Position the icon above the workstation
            UpdateIconPosition(workstation.gameObject);
        }
    }
}