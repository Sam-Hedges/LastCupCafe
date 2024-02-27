using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerSelectMenuHandler : MonoBehaviour {
    
    [Header("Cards")] [Tooltip("")] [SerializeField]
    private GameObject playerCardPrefab;

    [Tooltip("")] [SerializeField] private GameObject addPlayerCard;

    [Tooltip("")] [SerializeField] private CharacterCardPreset[] _characterCardPresets;

    [SerializeField] private Sprite gamepadIcon;
    [SerializeField] private Sprite keyboardIcon;


    [Header("Listening on Channels")]
    [Tooltip("Receives a reference of an Input Controller when it is spawned")]
    [SerializeField]
    private GameObjectEventChannelSO inputControllerInstancedChannel;

    [Tooltip("Receives a reference of an Input Controller when it is destroyed")] [SerializeField]
    private GameObjectEventChannelSO inputControllerDestroyedChannel;


    [Header("Runtime Anchors")] [Tooltip("")] [SerializeField]
    private InputControllerManagerAnchor inputControllerManagerAnchor;

    private InputControllerManager _inputControllerManager;

    private Dictionary<InputController, CharacterCardController> _playerInputToUI = new();

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

        // Set the character card transform parent to this transform and make it second from last in the hierarchy
        CharacterCardController playerCard = Instantiate(playerCardPrefab, transform).GetComponent<CharacterCardController>();
        playerCard.transform.SetSiblingIndex(transform.childCount - 2);
        
        // Input
        InputController inputController = go.GetComponent<InputController>();
        inputController.EnableMenuInput();
        playerCard.InputController = inputController;
        
        int randomIndex = UnityEngine.Random.Range(0, _characterCardPresets.Length - 1);
        playerCard.Preset = _characterCardPresets[randomIndex];
        playerCard.characterIndex = randomIndex;

        if (go.GetComponent<PlayerInput>().currentControlScheme == "Gamepad") {
            playerCard.inputTypeImage.sprite = gamepadIcon;
        }
        else {
            playerCard.inputTypeImage.sprite = keyboardIcon;
        }

        _playerInputToUI.Add(inputController, playerCard);
    }

    private void InputControllerDestroyed(GameObject go) {
        var inputController = go.GetComponent<InputController>();
        if (_playerInputToUI.TryGetValue(inputController, out CharacterCardController characterCard)) {
            Destroy(characterCard.gameObject);
            _playerInputToUI.Remove(inputController);
        }
    }
    
    public void IncrementPreset(CharacterCardController characterCard) { 
        if (characterCard.characterIndex == _characterCardPresets.Length - 1) {
            characterCard.characterIndex = 0;
        }
        else {
            characterCard.characterIndex++;
        }
        characterCard.Preset = _characterCardPresets[characterCard.characterIndex];
    }

    public void DecrementPreset(CharacterCardController characterCard) {
        if (characterCard.characterIndex == 0) {
            characterCard.characterIndex = _characterCardPresets.Length - 1;
        }
        else {
            characterCard.characterIndex--;
        }
        characterCard.Preset = _characterCardPresets[characterCard.characterIndex];
    }
    
    private void InputControllerManagerProvided() {
        _inputControllerManager = inputControllerManagerAnchor.Value;
    }
}