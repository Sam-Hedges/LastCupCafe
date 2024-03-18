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
    
    [Header("Broadcasting on Channels")]
    [SerializeField] private LoadEventChannelSO loadGameplaySceneChannel;
    [SerializeField] private GameSceneSO gameplayScene;

    [Header("Runtime Anchors")] [Tooltip("")] [SerializeField]
    private InputControllerManagerAnchor inputControllerManagerAnchor;
    private InputControllerManager inputControllerManager;

    private Dictionary<InputController, CharacterCardController> _playerInputToUI = new();
    
    private Coroutine _countdownCoroutine;
    [Header("Countdown")] 
    [SerializeField] private GameObject countdownGameObject;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int countdownTimeSeconds = 5;
    

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

    private void Awake() {
        inputControllerManager = inputControllerManagerAnchor.Value;
    }
    
    private void Start() {
        if (inputControllerManager == null) return;
        foreach (var inputController in inputControllerManager.InputControllers) {
            InputControllerInstanced(inputController.gameObject);
        }
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
        
        UpdateAddPlayerCard();
    }

    private void InputControllerDestroyed(GameObject go) {
        var inputController = go.GetComponent<InputController>();
        if (_playerInputToUI.TryGetValue(inputController, out CharacterCardController characterCard)) {
            Destroy(characterCard.gameObject);
            _playerInputToUI.Remove(inputController);
        }
        
        UpdateAddPlayerCard();
    }
    
    private void UpdateAddPlayerCard() {
        if (_playerInputToUI.Count < 2) {
            addPlayerCard.SetActive(true);
        } else {
            addPlayerCard.SetActive(false);
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

    public void CheckPlayersReady() {
        foreach (var player in _playerInputToUI) {
            if (player.Value.isReady == false) {
                if (_countdownCoroutine != null) {
                    StopCoroutine(_countdownCoroutine);
                    countdownGameObject.SetActive(false);
                    _countdownCoroutine = null;
                }
                break;
            }
        }
       
        if (_countdownCoroutine == null) {
            _countdownCoroutine = StartCoroutine(Countdown());
        }
    }
    
    private IEnumerator Countdown() {
        countdownGameObject.SetActive(true);
        countdownText.text = countdownTimeSeconds.ToString();
        
        for (int i = countdownTimeSeconds; i > 0; i--) {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        
        loadGameplaySceneChannel.RaiseEvent(gameplayScene, true, true);
    }
    
    private void InputControllerManagerProvided() {
        inputControllerManager = inputControllerManagerAnchor.Value;
    }
}