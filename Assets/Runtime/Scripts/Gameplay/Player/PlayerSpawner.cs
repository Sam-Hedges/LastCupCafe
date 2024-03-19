using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Listening on Channels")]
    [SerializeField] private GameObjectEventChannelSO inputControllerInstancedChannel;
    
    [Header("Broadcasting on Channels")]
    [SerializeField] private VoidEventChannelSO spawnPlayerControllerChannel = default;
    [SerializeField] private TransformEventChannelSO setPlayerParentChannel = default;
    
    [Header("General")]
    [SerializeField] private Transform playerParent = default;

    private void OnEnable() {
        inputControllerInstancedChannel.OnEventRaised += InputControllerInstanced;
        StartCoroutine(SpawnPlayer(1));
    }

    private void OnDisable() {
        inputControllerInstancedChannel.OnEventRaised -= InputControllerInstanced;
    }
    
    private void InputControllerInstanced(GameObject inputControllerGameObject) {
        StartCoroutine(SpawnPlayer(5));
    }

    private IEnumerator SpawnPlayer(int secondsDelay) {
        yield return new WaitForSeconds(secondsDelay);
        spawnPlayerControllerChannel.RaiseEvent();
        setPlayerParentChannel.RaiseEvent(playerParent);
        yield return null;
    }
}
