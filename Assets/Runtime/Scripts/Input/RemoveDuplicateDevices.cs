using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class RemoveDuplicateDevices : MonoBehaviour {
    
    private PlayerInput playerInput;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>(); 
    }

    private void Start() {
        StartCoroutine(CheckForDuplicateDevices());
    }

    // This is a coroutine so it can run concurrently as it checks devices
    private IEnumerator CheckForDuplicateDevices() {
        // Remove Duplicate Switch Pro Controller caused by bt connection with 8bitdo adapter
        foreach (var device in playerInput.devices) {
            if (device is UnityEngine.InputSystem.Switch.SwitchProControllerHID) {
                foreach (var item in Gamepad.all) {
                    if (item is UnityEngine.InputSystem.XInput.XInputController &&
                        Math.Abs(item.lastUpdateTime - device.lastUpdateTime) < 0.2f) {
                        Debug.Log(
                            $"Switch Pro controller detected and a copy of XInput was active at almost the same time. Disabling XInput device. `{device}`; `{item}`");
                        InputSystem.RemoveDevice(device);
                        Destroy(gameObject);
                        yield return null;
                    }
                }
            }
        }
        
        // If no duplicate devices are found, destroy this script
        Destroy(this);
        yield return null;
    }
}
