using System;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterCardController : MonoBehaviour {
    [Header("References")] [Tooltip("")] [SerializeField]
    public Image nameBackground;

    [Tooltip("")] [SerializeField] public TextMeshProUGUI characterName;

    [Tooltip("")] [SerializeField] public RawImage characterImage;

    [Tooltip("")] [SerializeField] public Image inputTypeImage;

    [Tooltip("")] [SerializeField] public GameObject isNotReadyIconGameObject;

    [Tooltip("")] [SerializeField] public GameObject isReadyIconGameObject;

    public bool isReady = false;
    public int characterIndex;

    private PlayerSelectMenuHandler _playerSelectMenuHandler;

    private InputController inputController;

    public InputController InputController {
        get => inputController;
        set {
            if (inputController != null) inputController.MenuNavigateEvent -= OnNavigate;
            inputController = value;
            inputController.MenuNavigateEvent += OnNavigate;
            inputController.MenuInteractEvent += OnInteract;
            inputController.MenuCancelEvent += OnCancel;
        }
    }

    private CharacterCardPreset preset;

    public CharacterCardPreset Preset {
        get => preset;
        set {
            preset = value;
            characterName.text = preset.name;
            nameBackground.sprite = preset.titleSprite;
            characterImage.texture = preset.characterImage;
        }
    }

    private void OnDestroy() {
        InputController.MenuNavigateEvent -= OnNavigate;
        InputController.MenuInteractEvent -= OnInteract;
        InputController.MenuCancelEvent -= OnCancel;
    }

    private void Awake() {
        _playerSelectMenuHandler = transform.parent.GetComponent<PlayerSelectMenuHandler>();
    }

    private void OnNavigate(InputAction.CallbackContext context) {
        Vector2 value = context.ReadValue<Vector2>();

        if (!context.started || isReady) return;
        if (value.x > 0) {
            _playerSelectMenuHandler.IncrementPreset(this);
        }
        else if (value.x < 0) {
            _playerSelectMenuHandler.DecrementPreset(this);
        }
    }

    private void OnInteract() {
        isReady = true;
        isReadyIconGameObject.SetActive(true);
        isNotReadyIconGameObject.SetActive(false);
        _playerSelectMenuHandler.CheckPlayersReady();
    }

    private void OnCancel() {
        isReady = false;
        isReadyIconGameObject.SetActive(false);
        isNotReadyIconGameObject.SetActive(true);
        _playerSelectMenuHandler.CheckPlayersReady();
    }
}