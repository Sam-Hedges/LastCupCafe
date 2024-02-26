using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

/// <summary>
/// Input is handled by the InputHandler, which is a ScriptableObject that can be referenced by other classes.
/// This allows for the input to be easily referenced by classes across scenes
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour, UserActions.IGameplayActions, UserActions.IUIActions {
    [Header("Broadcasting on Channels")]
    [Tooltip("Sends a reference of itself to the Input Controller Manager when it is spawned")]
    [SerializeField]
    private GameObjectEventChannelSO _InputControllerInstancedChannel = default;

    [Tooltip("Sends a reference of itself to the Input Controller Manager when it is destroyed")] [SerializeField]
    private GameObjectEventChannelSO _InputControllerDestroyedChannel = default;

    private PlayerController _parentPlayerController;
    private UserActions _userActions; // Actions Asset
    private PlayerInput _playerInput; // Player Input Component
    
    private InputSystemUIInputModule _uiModule;
    private MultiplayerEventSystem _eventSystem;
    
    #region Control Flow Methods

    private void Awake() {
        _uiModule = GetComponent<InputSystemUIInputModule>();
        _eventSystem = GetComponent<MultiplayerEventSystem>();
        _playerInput = GetComponent<PlayerInput>();
        
        _userActions = new UserActions();

        _InputControllerInstancedChannel.RaiseEvent(this.gameObject);
        
        _eventSystem.playerRoot = FindObjectOfType<Canvas>().gameObject;
        _playerInput.uiInputModule = _uiModule;
        _uiModule.actionsAsset = _playerInput.actions;
        ReassignActions();
    }

    private void Start() {
        
    }

    private void OnDestroy() {
        _InputControllerDestroyedChannel.RaiseEvent(this.gameObject);
    }

    private void OnEnable() {
        EnableGameplayInput();
    }

    private void OnDisable() {
        DisableAllInput();
    }

    public void EnableGameplayInput() {
        DisableAllInput();
        _userActions.Gameplay.Enable();
    }

    public void EnableMenuInput() {
        DisableAllInput();
        _userActions.UI.Enable();
    }

    public void DisableAllInput() {
        _userActions.Gameplay.Disable();
        _userActions.UI.Disable();
    }

    public void ToggleInputMap() {
        if (_playerInput.currentActionMap.name == "Gameplay") {
            _userActions.UI.Enable();
            _userActions.Gameplay.Disable();
        } else {
            _userActions.Gameplay.Enable();
            _userActions.UI.Disable();
        }
    }

    public PlayerController GetPlayerController() {
        return _parentPlayerController;
    }

    public void SetPlayerController(PlayerController playerController) {
        _parentPlayerController = playerController;
    }

    #endregion

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

    public void OnMovement(InputAction.CallbackContext context) {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
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
        
        _eventSystem.playerRoot = gameObject;
        _playerInput.uiInputModule = _uiModule;
        ReassignActions();

        // if (context.phase == InputActionPhase.Performed)
        //     PauseEvent?.Invoke();
    }

    #endregion

    #region Menu

    // Event handlers for the Menu action map
    // Assign delegate{} to events to initialise them with an empty delegate
    // so we can skip the null check when we use them
    public event UnityAction<InputController> AnyInputEvent = delegate { };
    public event UnityAction MoveSelectionEvent = delegate { };
    public event UnityAction MenuMouseMoveEvent = delegate { };
    public event UnityAction MenuClickButtonEvent = delegate { };
    public event UnityAction MenuUnpauseEvent = delegate { };
    public event UnityAction MenuCloseEvent = delegate { };
    public event UnityAction OpenInventoryEvent = delegate { }; // Used to bring up the inventory
    public event UnityAction CloseInventoryEvent = delegate { }; // Used to bring up the inventory
    public event UnityAction<float> TabSwitched = delegate { };
    public event UnityAction InventoryActionButtonEvent = delegate { };
    public event UnityAction SaveActionButtonEvent = delegate { };

    public void OnOpenInventory(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            OpenInventoryEvent.Invoke();
    }

    public void OnCancel(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuCloseEvent.Invoke();
    }

    public void OnInventoryActionButton(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            InventoryActionButtonEvent.Invoke();
    }

    public void OnSaveActionButton(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            SaveActionButtonEvent.Invoke();
    }

    public void OnResetActionButton(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            PauseEvent.Invoke();
    }

    public void OnMoveSelection(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MoveSelectionEvent.Invoke();
    }

    public void OnConfirm(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuClickButtonEvent.Invoke();
    }


    public void OnMouseMove(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuMouseMoveEvent.Invoke();
    }

    public void OnUnpause(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuUnpauseEvent.Invoke();
    }

    public void OnChangeTab(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            TabSwitched.Invoke(context.ReadValue<float>());
    }

    public bool LeftMouseDown() => Mouse.current.leftButton.isPressed;

    public void OnClick(InputAction.CallbackContext context) { }

    public void OnSubmit(InputAction.CallbackContext context) { }

    public void OnPoint(InputAction.CallbackContext context) { }

    public void OnRightClick(InputAction.CallbackContext context) { }

    public void OnNavigate(InputAction.CallbackContext context) { }

    public void OnCloseInventory(InputAction.CallbackContext context) {
        CloseInventoryEvent.Invoke();
    }

    public void OnAnyInput(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            AnyInputEvent?.Invoke(this);
    }

    public void OnScrollWheel(InputAction.CallbackContext context) { }

    public void OnMiddleClick(InputAction.CallbackContext context) { }

    #endregion

    private void ReassignActions() {
        _uiModule.point = InputActionReference.Create(_userActions.UI.Point);
        _uiModule.leftClick = InputActionReference.Create(_userActions.UI.Click);
        _uiModule.rightClick = InputActionReference.Create(_userActions.UI.RightClick);
        _uiModule.middleClick = InputActionReference.Create(_userActions.UI.MiddleClick);
        _uiModule.scrollWheel = InputActionReference.Create(_userActions.UI.ScrollWheel);
        _uiModule.move = InputActionReference.Create(_userActions.UI.Navigate);
        _uiModule.submit = InputActionReference.Create(_userActions.UI.Submit);
        _uiModule.cancel = InputActionReference.Create(_userActions.UI.Cancel);
    }
}