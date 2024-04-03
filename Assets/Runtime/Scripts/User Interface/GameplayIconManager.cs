using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayIconManager : MonoBehaviour
{
    [Header("Listening on Channels")]
    [SerializeField] private WorkstationStateEventChannelSO _workstationStateUpdateChannel;
    
    [Header("Icon Settings")]
    [SerializeField] private float iconHeight = 1;
    [SerializeField] private float iconScale = 1;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Sprite emptySlotIcon;
    private Dictionary<Workstation, GameObject> _workstationIcons = new Dictionary<Workstation, GameObject>();

    private void Awake()
    {
        _workstationStateUpdateChannel.OnEventRaised += UpdateIcon;
    }

    private void OnDestroy()
    {
        _workstationStateUpdateChannel.OnEventRaised -= UpdateIcon;
    }

    private void UpdateIcon(Workstation workstation)
    {
        // Check if the workstation already has an icon
        if (!_workstationIcons.TryGetValue(workstation, out GameObject icon))
        {
            // Create icon if not exist
            icon = Instantiate(iconPrefab, transform);
            _workstationIcons[workstation] = icon;
        }

        // Update the icon based on the workstation's state and contained item
        if (!workstation.currentlyStoredItem)
        {
            icon.GetComponent<Image>().sprite = emptySlotIcon;
        }
        else if (workstation.currentlyStoredItem != null)
        {
            icon.GetComponent<Image>().sprite = workstation.currentlyStoredItem.iconSprite;
        }
        // Additional conditions for minigame start, success, failure can be added here
        
        // Position the icon above the workstation
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(workstation.transform.position + new Vector3(0, iconHeight, 0.25f));
        icon.transform.position = screenPoint;
        icon.transform.localScale = Vector3.one * iconScale;
    }
}
