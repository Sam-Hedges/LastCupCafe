using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

/// <summary>
/// Input is handled by the InputHandler, which is a ScriptableObject that can be referenced by other classes.
/// This allows for the input to be easily referenced by classes across scenes
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour, UserActions.IGameplayActions, UserActions.IUIActions
{
    [Header("Broadcasting on Channels")]
    [Tooltip("Sends a reference of itself to the Input Controller Manager when it is spawned")]
    [SerializeField]
    private GameObjectEventChannelSO inputControllerInstancedChannel = default;

    [Tooltip("Sends a reference of itself to the Input Controller Manager when it is destroyed")] [SerializeField]
    private GameObjectEventChannelSO inputControllerDestroyedChannel = default;

    [Header("Anchors")] [Tooltip("Used to set this input controllers selected UI element")] [SerializeField]
    private GameObjectAnchor selectedGameObjectAnchor = default;

    private MultiplayerEventSystem _eventSystem;
    private PlayerInput _playerInput;

    public PlayerController PlayerController { get; set; }
    [HideInInspector] public int PlayerModelIndex { get; private set; } = 1;

    private void Awake() {
        // Player Input
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.uiInputModule = GetComponent<InputSystemUIInputModule>();
        _playerInput.uiInputModule.actionsAsset = _playerInput.actions;

        // Event System
        _eventSystem = GetComponent<MultiplayerEventSystem>();
        if (selectedGameObjectAnchor.Value != null) {
            _eventSystem.SetSelectedGameObject(selectedGameObjectAnchor.Value);
        }

        // Events and Anchors
        selectedGameObjectAnchor.OnAnchorProvided += SetSelectedGameObject;
        inputControllerInstancedChannel.RaiseEvent(gameObject);
    }

    private void OnDestroy() {
        inputControllerDestroyedChannel.RaiseEvent(gameObject);
        selectedGameObjectAnchor.OnAnchorProvided -= SetSelectedGameObject;
    }

    private void SetSelectedGameObject() {
        _eventSystem.SetSelectedGameObject(selectedGameObjectAnchor.Value);
    }

    public void EnableGameplayInput() {
        _playerInput.SwitchCurrentActionMap("Gameplay");
    }

    public void EnableMenuInput() {
        _playerInput.SwitchCurrentActionMap("UI");
    }
    
    public void SetPlayerModelIndex(int index) {
        PlayerModelIndex = index;
    }

    #region Gameplay

    // Event handlers for the Gameplay action map
    // Assign delegate{} to events to initialise them with an empty delegate
    // so we can skip the null check when we use them
    public event UnityAction DashEvent = delegate { };
    public event UnityAction StationInteractEvent = delegate { };

    public event UnityAction ThrowEvent = delegate { };

    public event UnityAction PauseEvent = delegate { };
    public event UnityAction ItemInteractEvent = delegate { };
    public event UnityAction EmoteEvent = delegate { };
    public event UnityAction<Vector2> MoveEvent = delegate { };
    public event UnityAction<Vector2> MinigameMoveEvent = delegate { };
    public event UnityAction<float> PressureEvent = delegate { };

    public void OnMovement(InputAction.CallbackContext context) {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
    
    public void OnMinigameMovement(InputAction.CallbackContext context)
    {
        MinigameMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPressureEvent(InputAction.CallbackContext context)
    {
        PressureEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnStationInteract(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started)
            StationInteractEvent?.Invoke();
        else if (context.phase == InputActionPhase.Canceled)
            ThrowEvent?.Invoke();
    }

    public void OnDash(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            DashEvent?.Invoke();
    }

    public void OnItemInteract(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            ItemInteractEvent?.Invoke();
        }
    }

    public void OnEmote(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            EmoteEvent?.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context) {
        // _uiModule.actionsAsset = _playerInput.actions;

        // eventSystem.playerRoot = gameObject;
        // playerInput.uiInputModule = uiModule;

        // if (context.phase == InputActionPhase.Performed)
        //     PauseEvent?.Invoke();
    }

    #endregion

    #region Menu

    // Event handlers for the Menu action map
    // Assign delegate{} to events to initialise them with an empty delegate
    // so we can skip the null check when we use them
    public event UnityAction<InputAction.CallbackContext> MenuNavigateEvent = delegate { };
    public event UnityAction<Vector2> MenuScrollEvent = delegate { };
    public event UnityAction MenuMouseMoveEvent = delegate { };
    public event UnityAction MenuInteractEvent = delegate { };
    public event UnityAction MenuRightMouseEvent = delegate { };
    public event UnityAction MenuMiddleMouseEvent = delegate { };
    public event UnityAction MenuCancelEvent = delegate { };
    public event UnityAction<float> TabSwitched = delegate { };

    public void OnAnyInput(InputAction.CallbackContext context) {
    }

    public void OnPoint(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuMouseMoveEvent?.Invoke();
    }

    public void OnClick(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuInteractEvent?.Invoke();
    }

    public void OnMiddleClick(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuMiddleMouseEvent?.Invoke();
    }

    public void OnRightClick(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuRightMouseEvent?.Invoke();
    }

    public void OnScrollWheel(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuScrollEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnNavigate(InputAction.CallbackContext context) {
        MenuNavigateEvent?.Invoke(context);
        Debug.Log("Navigate");
    }

    public void OnSubmit(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuInteractEvent?.Invoke();
    }

    public void OnCancel(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuCancelEvent?.Invoke();
    }

    #endregion
}