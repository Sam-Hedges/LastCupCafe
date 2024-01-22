using System;   
using UnityEngine;
using UnityEngine.Events;

public class MenuCloseEventHandler : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [Space]
    [SerializeField] private UnityEvent backEvent;

    private void OnEnable() {
        inputHandler.MenuCloseEvent += MenuClose;
    }
    
    private void OnDisable() {
        inputHandler.MenuCloseEvent -= MenuClose;
    }
    
    internal void MenuClose() {
        backEvent.Invoke();
        inputHandler.MenuCloseEvent -= MenuClose;
    }
}