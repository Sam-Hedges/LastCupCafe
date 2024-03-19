using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HandleLocalPlayersUI : MonoBehaviour {
    [Header("UI Elements")] [Tooltip("Local Player UI Prefab")] [SerializeField]
    private GameObject prefab;

    [SerializeField] private Sprite gamepadIcon;
    [SerializeField] private Sprite keyboardIcon;

    [Header("Listening on Channels")]
    [Tooltip("Receives a reference of an Input Controller when it is spawned")]
    [SerializeField]
    private GameObjectEventChannelSO inputControllerInstancedChannel;
    [Tooltip("Receives a reference of an Input Controller when it is destroyed")]
    [SerializeField]
    private GameObjectEventChannelSO inputControllerDestroyedChannel;
    
    private Dictionary<PlayerInput, GameObject> _playerInputToUI = new Dictionary<PlayerInput, GameObject>();

    private void OnEnable() {
        inputControllerInstancedChannel.OnEventRaised += AddLocalPlayerUI;
        inputControllerDestroyedChannel.OnEventRaised += RemoveLocalPlayerUI;
    }

    private void OnDisable() {
        inputControllerInstancedChannel.OnEventRaised -= AddLocalPlayerUI;
        inputControllerDestroyedChannel.OnEventRaised -= RemoveLocalPlayerUI;
    }
    
    private void RemoveLocalPlayerUI(PlayerInput playerInput) {
        if (_playerInputToUI.TryGetValue(playerInput, out GameObject localPlayerUI)) {
            Destroy(localPlayerUI);
            _playerInputToUI.Remove(playerInput);
        }
    }

    private void AddLocalPlayerUI(GameObject go) {
        PlayerInput playerInput = go.GetComponent<PlayerInput>();
        GameObject newLocalPlayerUI = Instantiate(prefab, transform);
        playerInput.onDeviceLost += RemoveLocalPlayerUI;
        
        _playerInputToUI.Add(playerInput, newLocalPlayerUI);
        
        foreach (Transform child in newLocalPlayerUI.transform) {
            if (child.name == "Icon") {
                if (go.GetComponent<PlayerInput>().currentControlScheme == "Gamepad") {
                    child.GetComponent<Image>().sprite = gamepadIcon;
                }
                else {
                    child.GetComponent<Image>().sprite = keyboardIcon;
                }
            }
        }

        newLocalPlayerUI.GetComponentInChildren<TextMeshProUGUI>().text = "Player " + transform.childCount;
    }
    
    private void RemoveLocalPlayerUI(GameObject go) {
        PlayerInput playerInput = go.GetComponent<PlayerInput>();
        if (_playerInputToUI.TryGetValue(playerInput, out GameObject localPlayerUI)) {
            Destroy(localPlayerUI);
            _playerInputToUI.Remove(playerInput);
        }
    }
}