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
    [SerializeField] private GameObject gridBaseIcons;

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
        var deleteQueue = new List<KeyValuePair<GameObject, GameObject>>();
        foreach (var keyValuePair in _gameplayIcons) {
            if (!keyValuePair.Key)
            {
                deleteQueue.Add(keyValuePair);
                continue;
            }
            if (keyValuePair.Key.activeSelf != keyValuePair.Value.activeSelf) {
                keyValuePair.Value.SetActive(keyValuePair.Key.activeSelf);
            }
            UpdateIconPosition(keyValuePair.Key);
        }

        foreach (var keyValuePair in deleteQueue) {
            _gameplayIcons.Remove(keyValuePair.Key);
            Destroy(keyValuePair.Value);
            Destroy(keyValuePair.Key);
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
            CheckForIcon(go, gridBaseIcons);
            UpdateIconPosition(mug.gameObject);
            mug.SetIconGridHandler(_gameplayIcons[go].GetComponent<IconGridHandler>());
            return;
        }

        if (go.TryGetComponent(out Ingredient ingredient)) {
            switch (ingredient.IngredientType) {
                case IngredientType.Milk:
                    CheckForIcon(go, defaultBaseIcon);
                    UpdateIconPosition(ingredient.gameObject);
                    Instantiate(milkIcon, _gameplayIcons[go].transform, false);
                    return;
                case IngredientType.CoffeeBeans:
                    CheckForIcon(go, defaultBaseIcon);
                    UpdateIconPosition(ingredient.gameObject);
                    Instantiate(coffeeBeansIcon, _gameplayIcons[go].transform, false);
                    return;
                case IngredientType.CoffeeGrounds:
                    CheckForIcon(go, defaultBaseIcon);
                    UpdateIconPosition(ingredient.gameObject); 
                    Instantiate(coffeeGroundsIcon, _gameplayIcons[go].transform, false);
                    return;
                case IngredientType.ChocolatePowder:
                    CheckForIcon(go, defaultBaseIcon);
                    UpdateIconPosition(ingredient.gameObject);
                    Instantiate(chocolateIcon, _gameplayIcons[go].transform, false);
                    return;
                case IngredientType.CaramelSyrup:
                    CheckForIcon(go, defaultBaseIcon);
                    UpdateIconPosition(ingredient.gameObject);
                    Instantiate(caramelIcon, _gameplayIcons[go].transform, false);
                    return;
                case IngredientType.Water:
                    // Can't hold water directly
                    return;
            }
        }

        if (go.TryGetComponent(out Workstation workstation)) {
            
            switch (workstation) {
                case Kettle:
                    break;
                case CoffeeMachine:
                    CheckForIcon(go, coffeeMachineUI);
                    UpdateIconPosition(workstation.gameObject);
                    // Disgusting Code
                    workstation.GetComponent<CoffeeMachine>().Icons = _gameplayIcons[go].GetComponent<CoffeeMachineMG>();
                    return;
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