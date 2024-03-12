using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// Input is handled by the InputHandler, which is a ScriptableObject that can be referenced by other classes.
/// This allows for the input to be easily referenced by classes across scenes
/// </summary>
[CreateAssetMenu(fileName = "InputHandler", menuName = "Input Handler")]
public class InputHandler : SerializableScriptableObject, UserActions.IGameplayActions/*, UserActions.IUIActions*/ {
    // The Input Actions asset
    private UserActions _userActions;

    #region Control Flow Methods

    private void OnEnable() {
        if (_userActions == null) {
            // Create the actions asset
            _userActions = new UserActions();

            // Assign the callbacks for the action maps
            // _userActions.UI.SetCallbacks(this);
            // _userActions.Gameplay.SetCallbacks(this);
        }
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
        // _userActions.UI.Enable();
    }

    public void DisableAllInput() {
        _userActions.Gameplay.Disable();
        // _userActions.UI.Disable();
    }

    #endregion

    #region Gameplay

    // Event handlers for the Gameplay action map
    // Assign delegate{} to events to initialise them with an empty delegate
    // so we can skip the null check when we use them
    public event UnityAction DodgeEvent = delegate { };
    public event UnityAction AttackEvent = delegate { };
    public event UnityAction AttackCanceledEvent = delegate { };
    public event UnityAction SkillEvent_01 = delegate { };

    public event UnityAction
        InteractEvent = delegate { }; // Used to talk, pickup objects, interact with tools like the cooking cauldron

    public event UnityAction InventoryActionButtonEvent = delegate { };
    public event UnityAction SaveActionButtonEvent = delegate { };
    public event UnityAction ResetActionButtonEvent = delegate { };
    public event UnityAction<Vector2> MoveEvent = delegate { };
    public event UnityAction<Vector2> MinigameMoveEvent = delegate { };
    public event UnityAction<Vector2> LookEvent = delegate { };

    public void OnMovement(InputAction.CallbackContext context) {
        MoveEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnStationInteract(InputAction.CallbackContext context) {
        throw new System.NotImplementedException();
    }

    public void OnDash(InputAction.CallbackContext context) {
        throw new System.NotImplementedException();
    }

    public void OnLook(InputAction.CallbackContext context) {
        LookEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnAttack(InputAction.CallbackContext context) {
        switch (context.phase) {
            case InputActionPhase.Performed:
                AttackEvent.Invoke();
                break;
            case InputActionPhase.Canceled:
                AttackCanceledEvent.Invoke();
                break;
        }
    }

    public void OnDodge(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            DodgeEvent.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context) {
        if ((context.phase == InputActionPhase.Performed)) {
            //&& (_gameStateManager.CurrentGameState == GameState.Gameplay)) // Interaction is only possible when in gameplay GameState
            InteractEvent.Invoke();
        }
    }

    public void OnSkill_01(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            SkillEvent_01.Invoke();
    }

    public void OnMinigameMovement(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            MinigameMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }
    }

    #endregion

    #region Menu

    // Event handlers for the Menu action map
    // Assign delegate{} to events to initialise them with an empty delegate
    // so we can skip the null check when we use them
    public event UnityAction StartGameEvent = delegate { };
    public event UnityAction MoveSelectionEvent = delegate { };
    public event UnityAction MenuMouseMoveEvent = delegate { };
    public event UnityAction MenuClickButtonEvent = delegate { };
    public event UnityAction MenuUnpauseEvent = delegate { };
    public event UnityAction MenuPauseEvent = delegate { };
    public event UnityAction MenuCloseEvent = delegate { };
    public event UnityAction OpenInventoryEvent = delegate { }; // Used to bring up the inventory
    public event UnityAction CloseInventoryEvent = delegate { }; // Used to bring up the inventory
    public event UnityAction<float> TabSwitched = delegate { };

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
            ResetActionButtonEvent.Invoke();
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

    public void OnPause(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuPauseEvent.Invoke();
    }

    public void OnItemInteract(InputAction.CallbackContext context) {
        throw new System.NotImplementedException();
    }

    public void OnEmote(InputAction.CallbackContext context) {
        throw new System.NotImplementedException();
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

    public void OnStartGame(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            StartGameEvent.Invoke();
    }

    public void OnScrollWheel(InputAction.CallbackContext context) { }

    public void OnMiddleClick(InputAction.CallbackContext context) { }

    #endregion
}