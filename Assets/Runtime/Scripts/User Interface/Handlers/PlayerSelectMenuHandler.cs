using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSelectMenuHandler : MonoBehaviour
{
    [Header("Cards")]
    [Tooltip("")] [SerializeField]
    private GameObject playerCardPrefab;
    
    [Tooltip("")] [SerializeField]
    private GameObject addPlayerCard;
    
    
    [Header("Listening on Channels")]
    [Tooltip("Receives a reference of an Input Controller when it is spawned")] [SerializeField]
    private GameObjectEventChannelSO inputControllerInstancedChannel;
    
    [Tooltip("Receives a reference of an Input Controller when it is destroyed")] [SerializeField]
    private GameObjectEventChannelSO inputControllerDestroyedChannel;
    
    
    [Header("Runtime Anchors")]
    [Tooltip("")] [SerializeField]
    private InputControllerManagerAnchor inputControllerManagerAnchor;

    private InputControllerManager _inputControllerManager;
    private Dictionary<InputController, GameObject> _playerInputToUI = new Dictionary<InputController, GameObject>();
    
    private void OnEnable() {
        inputControllerManagerAnchor.OnAnchorProvided += InputControllerManagerProvided;
        inputControllerInstancedChannel.OnEventRaised += InputControllerInstanced;
        inputControllerDestroyedChannel.OnEventRaised += InputControllerDestroyed;
    }

    private void OnDisable() {
        inputControllerManagerAnchor.OnAnchorProvided -= InputControllerManagerProvided;
        inputControllerInstancedChannel.OnEventRaised -= InputControllerInstanced;
        inputControllerDestroyedChannel.OnEventRaised -= InputControllerDestroyed;
    }

    private void InputControllerInstanced(GameObject go) {
        InputController inputController = go.GetComponent<InputController>();
        GameObject newLocalPlayerUI = Instantiate(playerCardPrefab, transform);
        
        _playerInputToUI.Add(inputController, newLocalPlayerUI);
        
        foreach (Transform child in newLocalPlayerUI.transform) {
            if (child.name == "Icon") {
                if (go.GetComponent<PlayerInput>().currentControlScheme == "Gamepad") {
                    // child.GetComponent<Image>().sprite = gamepadIcon;
                }
                else {
                    // child.GetComponent<Image>().sprite = keyboardIcon;
                }
            }
        }

        newLocalPlayerUI.GetComponentInChildren<TextMeshProUGUI>().text = "Player " + transform.childCount;
    }
    
    private void InputControllerDestroyed(GameObject go) {
        var inputController = go.GetComponent<InputController>();
        if (_playerInputToUI.TryGetValue(inputController, out GameObject localPlayerUI)) {
            Destroy(localPlayerUI);
            _playerInputToUI.Remove(inputController);
        }
    }

    private void InputControllerManagerProvided() {
        _inputControllerManager = inputControllerManagerAnchor.Value;
    }
}
