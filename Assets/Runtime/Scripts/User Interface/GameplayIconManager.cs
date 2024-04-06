using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayIconManager : MonoBehaviour
{
    [Header("Listening on Channels")]
    [SerializeField] private GameObjectEventChannelSO _gameobjectStateUpdateChannel;
    
    [Header("Broadcasting on Channels")]
	[SerializeField] private GameObjectEventChannelSO _iconCanvasReadyChannel;
    
    [Header("Icon Settings")]
    [SerializeField] private float iconHeight = 1;
    [SerializeField] private float iconScale = 1;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Sprite emptySlotIcon;
    private Dictionary<GameObject, GameObject> _workstationIcons = new Dictionary<GameObject, GameObject>();

    private void Awake()
    {
        _gameobjectStateUpdateChannel.OnEventRaised += UpdateIcon;
        _iconCanvasReadyChannel.RaiseEvent(gameObject);
    }

    private void OnDestroy()
    {
        _gameobjectStateUpdateChannel.OnEventRaised -= UpdateIcon;
    }

    private void Update() {
        foreach (var keyValuePair in _workstationIcons) {
            if (keyValuePair.Key.TryGetComponent(out Workstation workstation)) {
                switch (workstation) {
                    case Kettle:
                        break;
                    case CoffeeMachine:
                        break;
                }
                continue;
            }
            if (keyValuePair.Key.TryGetComponent(out Item item)) {
                switch (item) {
                    case Mug:
                        break;
                }
                
            }
        }
    }

    public void UpdateIcon(GameObject go)
    {
        if (go.TryGetComponent(out Item item)) {

            return;
        }
        
        if (go.TryGetComponent(out Workstation workstation)) {
            
            // Check if the workstation already has an icon
            if (!_workstationIcons.TryGetValue(go, out GameObject icon)) {
                // Create icon if not exist
                icon = Instantiate(iconPrefab, transform);
                _workstationIcons[go] = icon;
            }

            // Update the icon based on the workstation's state and contained item
            if (!workstation.currentlyStoredItem) {
                icon.GetComponent<Image>().sprite = emptySlotIcon;
            }
            else if (workstation.currentlyStoredItem != null) {
                icon.GetComponent<Image>().sprite = workstation.currentlyStoredItem.iconSprite;
            }
            // Additional conditions for minigame start, success, failure can be added here

            // Position the icon above the workstation
            Vector2 screenPoint =
                Camera.main.WorldToScreenPoint(workstation.transform.position + new Vector3(0, iconHeight, 0.25f));
            icon.transform.position = screenPoint;
            icon.transform.localScale = Vector3.one * iconScale;
        }
    }
}
