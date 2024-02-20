using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class RemoveSwitchProControllers : MonoBehaviour {
    
    private PlayerInput playerInput;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>(); 
    }

    private void Update() {
        foreach (var device in playerInput.devices) {
            if (device is UnityEngine.InputSystem.Switch.SwitchProControllerHID) {
                foreach (var item in Gamepad.all) {
                    if (item is UnityEngine.InputSystem.XInput.XInputController &&
                        Math.Abs(item.lastUpdateTime - device.lastUpdateTime) < 0.2f) {
                        Debug.Log(
                            $"Switch Pro controller detected and a copy of XInput was active at almost the same time. Disabling XInput device. `{device}`; `{item}`");
                        InputSystem.RemoveDevice(device);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
