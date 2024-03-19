using System;   
using UnityEngine;
using UnityEngine.Events;

public class MenuCloseEventHandler : MonoBehaviour
{
    [SerializeField] private InputControllerManagerAnchor inputControllerManagerAnchor;
    [Space]
    [SerializeField] private UnityEvent backEvent;

    private InputController _inputController;
    private void OnEnable() {
        inputControllerManagerAnchor.OnAnchorProvided += UpdateCurrentInputController;
    }
    
    private void OnDisable() {
        if (_inputController == null) return;
        _inputController.MenuCancelEvent -= MenuClose;
    }
    
    private void UpdateCurrentInputController() {
        if (_inputController != null) _inputController.MenuCancelEvent -= MenuClose;
        _inputController = inputControllerManagerAnchor.Value.leaderInputController;
        _inputController.MenuCancelEvent += MenuClose;
    }
    
    internal void MenuClose() {
        backEvent.Invoke();
        _inputController.MenuCancelEvent -= MenuClose;
    }
}