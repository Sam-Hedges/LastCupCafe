using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSelectMenuHandler : MonoBehaviour {
    [Serializable]
    private class CharacterCardPreset {
        public CharacterCard characterCard;
        public string name;
        public Sprite titleSprite;
        public RenderTexture characterImage;
        public bool isReady;
        public int characterIndex;
    }

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

    private Dictionary<InputController, CharacterCardPreset> _playerInputToUI = new();

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

        int randomIndex = UnityEngine.Random.Range(0, _characterCardPresets.Length);
        CharacterCardPreset playerCard = _characterCardPresets[randomIndex];
        playerCard.characterCard = Instantiate(playerCardPrefab, transform).GetComponent<CharacterCard>();
        
        // Set the character card transform parent to this transform and make it second from last in the hierarchy
        playerCard.characterCard.transform.SetParent(transform);
        playerCard.characterCard.transform.SetSiblingIndex(transform.childCount - 2);
        
        playerCard.isReady = false;
        playerCard.characterIndex = randomIndex;

        if (go.GetComponent<PlayerInput>().currentControlScheme == "Gamepad") {
            playerCard.characterCard._inputTypeImage.sprite = gamepadIcon;
        }
        else {
            playerCard.characterCard._inputTypeImage.sprite = keyboardIcon;
        }

        _playerInputToUI.Add(inputController, playerCard);
    }

    private void InputControllerDestroyed(GameObject go) {
        var inputController = go.GetComponent<InputController>();
        if (_playerInputToUI.TryGetValue(inputController, out CharacterCardPreset characterCardPreset)) {
            Destroy(characterCardPreset.characterCard.gameObject);
            _playerInputToUI.Remove(inputController);
        }
    }

    private void InputControllerManagerProvided() {
        _inputControllerManager = inputControllerManagerAnchor.Value;
    }
}