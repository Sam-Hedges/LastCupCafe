using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

/// <summary>
/// Input is handled by the InputHandler, which is a ScriptableObject that can be referenced by other classes.
/// This allows for the input to be easily referenced by classes across scenes
/// </summary>
public class PlayerInputHandler : MonoBehaviour, UserActions.IGameplayActions, UserActions.IUIActions
{
    // The Input Actions asset
    private UserActions _userActions;

    [SerializeField] private InputActionAsset m_Actions;

    private string m_DefaultControlScheme;
    private int m_PlayerIndex = -1;
    private InputUser m_InputUser;
    private static Action<InputUser, InputUserChange, InputDevice> s_UserChangeDelegate;

    private static int s_AllActivePlayersCount;
    private static PlayerInput[] s_AllActivePlayers;
    private static int s_InitPairWithDevicesCount;
    private static InputDevice[] s_InitPairWithDevices;
    private static int s_InitPlayerIndex = -1;
    private static string s_InitControlScheme;

    #region Control Flow Methods

    private void OnEnable() {
        if (_userActions == null) {
            // Create the actions asset
            _userActions = new UserActions();

            // Assign the callbacks for the action maps
            _userActions.UI.SetCallbacks(this);
            _userActions.Gameplay.SetCallbacks(this);
        }
        // Get InputActionMap from the _userActions asset

        AssignPlayerIndex();
        AssignUserAndDevices();


        // Add to global list and sort it by player index.
        ArrayHelpers.AppendWithCapacity(ref s_AllActivePlayers, ref s_AllActivePlayersCount, this);
        for (var i = 1; i < s_AllActivePlayersCount; ++i)
        for (var j = i; j > 0 && s_AllActivePlayers[j - 1].playerIndex > s_AllActivePlayers[j].playerIndex; --j)
            s_AllActivePlayers.SwapElements(j, j - 1);

        // If it's the first player, hook into user change notifications.
        if (s_AllActivePlayersCount == 1) {
            if (s_UserChangeDelegate == null)
                s_UserChangeDelegate = OnUserChange;
            InputUser.onChange += s_UserChangeDelegate;
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
        _userActions.UI.Enable();
    }

    public void DisableAllInput() {
        _userActions.Gameplay.Disable();
        _userActions.UI.Disable();
    }

    #endregion

    #region Input System Callbacks

    private void AssignPlayerIndex() {
        // If we have been given a specific player index, use that.
        if (s_InitPlayerIndex != -1)
            m_PlayerIndex = s_InitPlayerIndex;
        else { // Otherwise, find the next available index.
            int minPlayerIndex = int.MaxValue;
            int maxPlayerIndex = int.MinValue;

            // Find the range of indices in use
            for (var i = 0; i < s_AllActivePlayersCount; ++i) {
                int playerIndex = s_AllActivePlayers[i].playerIndex;
                minPlayerIndex = Math.Min(minPlayerIndex, playerIndex);
                maxPlayerIndex = Math.Max(maxPlayerIndex, playerIndex);
            }

            // If we have a gap between the minimum and maximum index, take the first one.
            if (minPlayerIndex != int.MaxValue && minPlayerIndex > 0) {
                // There's an index between 0 and the current minimum available.
                m_PlayerIndex = minPlayerIndex - 1;
            }
            else if (maxPlayerIndex != int.MinValue) {
                // There may be an index between the minimum and maximum available.
                // Search the range. If there's nothing, create a new maximum.
                for (var i = minPlayerIndex; i < maxPlayerIndex; ++i) {
                    if (GetPlayerByIndex(i) == null) {
                        m_PlayerIndex = i;
                        return;
                    }
                }

                m_PlayerIndex = maxPlayerIndex + 1;
            }
            else
                m_PlayerIndex = 0;
        }
    }

    public static PlayerInput GetPlayerByIndex(int playerIndex) {
        for (var i = 0; i < s_AllActivePlayersCount; ++i)
            if (s_AllActivePlayers[i].playerIndex == playerIndex)
                return s_AllActivePlayers[i];
        return null;
    }

    private void AssignUserAndDevices() {
        // If we already have a user at this point, clear out all its paired devices
        // to start the pairing process from scratch.
        if (m_InputUser.valid)
            m_InputUser.UnpairDevices();

        // All our input goes through actions so there's no point setting
        // anything up if we have none.
        if (m_Actions == null) {
            // If we have devices we are meant to pair with, do so.  Otherwise, don't
            // do anything as we don't know what kind of input to look for.
            if (s_InitPairWithDevicesCount > 0) {
                for (var i = 0; i < s_InitPairWithDevicesCount; ++i)
                    m_InputUser = InputUser.PerformPairingWithDevice(s_InitPairWithDevices[i], m_InputUser);
            }
            else {
                // Make sure user is invalid.
                m_InputUser = new InputUser();
            }

            return;
        }

        // If we have control schemes, try to find the one we should use.
        if (m_Actions.controlSchemes.Count > 0) {
            if (!string.IsNullOrEmpty(s_InitControlScheme)) {
                // We've been given a control scheme to initialize this. Try that one and
                // that one only. Might mean we end up with missing devices.

                var controlScheme = m_Actions.FindControlScheme(s_InitControlScheme);
                if (controlScheme == null) {
                    Debug.LogError($"No control scheme '{s_InitControlScheme}' in '{m_Actions}'", this);
                }
                else {
                    TryToActivateControlScheme(controlScheme.Value);
                }
            }
            else if (!string.IsNullOrEmpty(m_DefaultControlScheme)) {
                // There's a control scheme we should try by default.

                var controlScheme = m_Actions.FindControlScheme(m_DefaultControlScheme);
                if (controlScheme == null) {
                    Debug.LogError($"Cannot find default control scheme '{m_DefaultControlScheme}' in '{m_Actions}'",
                        this);
                }
                else {
                    TryToActivateControlScheme(controlScheme.Value);
                }
            }

            // If we did not end up with a usable scheme by now but we've been given devices to pair with,
            // search for a control scheme matching the given devices.
            if (s_InitPairWithDevicesCount > 0 && (!m_InputUser.valid || m_InputUser.controlScheme == null)) {
                // The devices we've been given may not be all the devices required to satisfy a given control scheme so we
                // want to pick any one control scheme that is the best match for the devices we have regardless of whether
                // we'll need additional devices. TryToActivateControlScheme will take care of that.
                var controlScheme = InputControlScheme.FindControlSchemeForDevices(
                    new ReadOnlyArray<InputDevice>(s_InitPairWithDevices, 0, s_InitPairWithDevicesCount),
                    m_Actions.controlSchemes,
                    allowUnsuccesfulMatch: true);
                if (controlScheme != null)
                    TryToActivateControlScheme(controlScheme.Value);
            }
            // If we don't have a working control scheme by now and we haven't been instructed to use
            // one specific control scheme, try each one in the asset one after the other until we
            // either find one we can use or run out of options.
            else if ((!m_InputUser.valid || m_InputUser.controlScheme == null) &&
                     string.IsNullOrEmpty(s_InitControlScheme)) {
                using (var availableDevices = InputUser.GetUnpairedInputDevices()) {
                    var controlScheme =
                        InputControlScheme.FindControlSchemeForDevices(availableDevices, m_Actions.controlSchemes);
                    if (controlScheme != null)
                        TryToActivateControlScheme(controlScheme.Value);
                }
            }
        }
        else {
            // There's no control schemes in the asset. If we've been given a set of devices,
            // we run with those (regardless of whether there's bindings for them in the actions or not).
            // If we haven't been given any devices, we go through all bindings in the asset and whatever
            // device is present that matches the binding and that isn't used by any other player, we'll
            // pair to the player.

            if (s_InitPairWithDevicesCount > 0) {
                for (var i = 0; i < s_InitPairWithDevicesCount; ++i)
                    m_InputUser = InputUser.PerformPairingWithDevice(s_InitPairWithDevices[i], m_InputUser);
            }
            else {
                // Pair all devices for which we have a binding.
                using (var availableDevices = InputUser.GetUnpairedInputDevices()) {
                    for (var i = 0; i < availableDevices.Count; ++i) {
                        var device = availableDevices[i];
                        if (!HaveBindingForDevice(device))
                            continue;

                        m_InputUser = InputUser.PerformPairingWithDevice(device, m_InputUser);
                    }
                }
            }
        }

        // If we don't have a valid user at this point, we don't have any paired devices.
        if (m_InputUser.valid)
            m_InputUser.AssociateActionsWithUser(m_Actions);
    }


    private bool TryToActivateControlScheme(InputControlScheme controlScheme) {
        // Pair any devices we may have been given.
        if (s_InitPairWithDevicesCount > 0) {
            // First make sure that all of the devices actually work with the given control scheme.
            // We're fine having to pair additional devices but we don't want the situation where
            // we have the player grab all the devices in s_InitPairWithDevices along with a control
            // scheme that fits none of them and then AndPairRemainingDevices() supplying the devices
            // actually needed by the control scheme.
            for (var i = 0; i < s_InitPairWithDevicesCount; ++i) {
                var device = s_InitPairWithDevices[i];
                if (!controlScheme.SupportsDevice(device))
                    return false;
            }

            // We're good. Give the devices to the user.
            for (var i = 0; i < s_InitPairWithDevicesCount; ++i) {
                var device = s_InitPairWithDevices[i];
                m_InputUser = InputUser.PerformPairingWithDevice(device, m_InputUser);
            }
        }

        if (!m_InputUser.valid)
            m_InputUser = InputUser.CreateUserWithoutPairedDevices();

        m_InputUser.ActivateControlScheme(controlScheme).AndPairRemainingDevices();
        if (m_InputUser.hasMissingRequiredDevices) {
            m_InputUser.ActivateControlScheme(null);
            m_InputUser.UnpairDevices();
            return false;
        }

        return true;
    }

    private bool HaveBindingForDevice(InputDevice device) {
        if (m_Actions == null)
            return false;

        var actionMaps = m_Actions.actionMaps;
        for (var i = 0; i < actionMaps.Count; ++i) {
            var actionMap = actionMaps[i];
            if (actionMap.IsUsableWithDevice(device))
                return true;
        }

        return false;
    }

    private static void OnUserChange(InputUser user, InputUserChange change, InputDevice device) {
        switch (change) {
            case InputUserChange.DeviceLost:
            case InputUserChange.DeviceRegained:
                for (var i = 0; i < s_AllActivePlayersCount; ++i) {
                    var player = s_AllActivePlayers[i];
                    if (player.m_InputUser == user) {
                        if (change == InputUserChange.DeviceLost)
                            player.HandleDeviceLost();
                        else if (change == InputUserChange.DeviceRegained)
                            player.HandleDeviceRegained();
                    }
                }

                break;

            case InputUserChange.ControlsChanged:
                for (var i = 0; i < s_AllActivePlayersCount; ++i) {
                    var player = s_AllActivePlayers[i];
                    if (player.m_InputUser == user)
                        player.HandleControlsChanged();
                }

                break;
        }
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
    public event UnityAction<Vector2> LookEvent = delegate { };

    public void OnMovement(InputAction.CallbackContext context) {
        MoveEvent.Invoke(context.ReadValue<Vector2>());
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

    public void OnUnpause(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            MenuUnpauseEvent.Invoke();
    }

    public void OnChangeTab(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            TabSwitched.Invoke(context.ReadValue<float>());
    }

    public bool LeftMouseDown() => Mouse.current.leftButton.isPressed;

    public void OnClick(InputAction.CallbackContext context) {
    }

    public void OnSubmit(InputAction.CallbackContext context) {
    }

    public void OnPoint(InputAction.CallbackContext context) {
    }

    public void OnRightClick(InputAction.CallbackContext context) {
    }

    public void OnNavigate(InputAction.CallbackContext context) {
    }

    public void OnCloseInventory(InputAction.CallbackContext context) {
        CloseInventoryEvent.Invoke();
    }

    public void OnStartGame(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed)
            StartGameEvent.Invoke();
    }

    public void OnScrollWheel(InputAction.CallbackContext context) {
    }

    public void OnMiddleClick(InputAction.CallbackContext context) {
    }

    #endregion
}