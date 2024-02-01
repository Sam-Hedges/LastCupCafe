using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AddLocalPlayersUI : MonoBehaviour {
    [Header("UI Elements")] [Tooltip("Local Player UI Prefab")] [SerializeField]
    private GameObject prefab;

    [SerializeField] private Sprite gamepadIcon;
    [SerializeField] private Sprite keyboardIcon;

    [Header("Listening on Channels")]
    [Tooltip("Receives a reference of an Input Controller when it is spawned")]
    [SerializeField]
    private GameObjectEventChannelSO inputControllerInstancedChannel;

    private void OnEnable() {
        inputControllerInstancedChannel.OnEventRaised += AddLocalPlayerUI;
    }

    private void OnDisable() {
        inputControllerInstancedChannel.OnEventRaised -= AddLocalPlayerUI;
    }

    private void AddLocalPlayerUI(GameObject go) {
        var newLocalPlayerUI = Instantiate(prefab, transform);
        
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
}