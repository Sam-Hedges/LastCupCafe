using System;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterCardController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("")] [SerializeField]
    public Image nameBackground;
    
    [Tooltip("")] [SerializeField]
    public TextMeshProUGUI characterName;

    [Tooltip("")] [SerializeField]
    public RawImage characterImage;
    
    [Tooltip("")] [SerializeField]
    public Image inputTypeImage;
    
    [Tooltip("")] [SerializeField]
    public GameObject isNotReadyIconGameObject;
    
    [Tooltip("")] [SerializeField]
    public GameObject isReadyIconGameObject;
    
    public bool isReady = false;
    public int characterIndex;
    
    private PlayerSelectMenuHandler playerSelectMenuHandler;

    private InputController inputController;
    public InputController InputController {
        get => inputController;
        set {
            if (inputController != null) inputController.MenuNavigateEvent -= OnNavigate;
            inputController = value;
            inputController.MenuNavigateEvent += OnNavigate;
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
    }

    private void Awake() {
        playerSelectMenuHandler = transform.parent.GetComponent<PlayerSelectMenuHandler>();
    }

    private void OnNavigate(Vector2 value) {
        Debug.Log("Navigating: " + value);
        if (value.x > 0) {
            playerSelectMenuHandler.IncrementPreset(this);
        }
        else if (value.x < 0) {
            playerSelectMenuHandler.DecrementPreset(this);
        }
    }
}
