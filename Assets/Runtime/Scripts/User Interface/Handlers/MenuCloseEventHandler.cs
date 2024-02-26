using System;   
using UnityEngine;
using UnityEngine.Events;

public class MenuCloseEventHandler : MonoBehaviour
{
    [SerializeField] private InputControllerManagerAnchor _inputControllerManagerAnchor;
    [Space]
    [SerializeField] private UnityEvent backEvent;

    private InputController _inputController;
    private void OnEnable() {
        _inputControllerManagerAnchor.OnAnchorProvided += UpdateCurrentInputController;
        UpdateCurrentInputController();
    }
    
    private void OnDisable() {
        if (_inputController == null) return;
        _inputController.MenuCloseEvent -= MenuClose;
    }
    
    private void UpdateCurrentInputController() {
        if (_inputController != null) _inputController.MenuCloseEvent -= MenuClose;
        _inputController = _inputControllerManagerAnchor.Value._leaderInputController;
        _inputController.MenuCloseEvent += MenuClose;
    }
    
    internal void MenuClose() {
        backEvent.Invoke();
        _inputController.MenuCloseEvent -= MenuClose;
    }
}