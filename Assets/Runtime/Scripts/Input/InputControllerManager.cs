using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class InputControllerManager : MonoBehaviour {
    
    [Header("Player Controller Pool")]
    [SerializeField] private PlayerControllerPoolSO playerControllerPool;
    [SerializeField] private int initialSize;

    [Header("Listening on Channels")]
    [Tooltip("Receives a reference of an Input Controller when it is spawned")] [SerializeField]
    private GameObjectEventChannelSO inputControllerInstancedChannel;
    
    [Tooltip("Receives a reference of an Input Controller when it is destroyed")] [SerializeField]
    private GameObjectEventChannelSO inputControllerDestroyedChannel;

    [Tooltip("Spawns a Player at the spawn point")] [SerializeField]
    private VoidEventChannelSO spawnPlayerControllerChannel;

    [Tooltip("Removes a Player from the scene")] [SerializeField]
    private PlayerControllerEventChannelSO despawnPlayerControllerChannel;

    [Tooltip("Sets the Player's parent to the PlayerParent object")] [SerializeField]
    private TransformEventChannelSO setPlayerControllerParentChannel;

    private List<InputController> _inputs;
    private List<PlayerController> _players;

    private PlayerInputManager _playerInputManager;

    private void Awake() {
        _playerInputManager = GetComponent<PlayerInputManager>();

        // Init Inputs
        _inputs = new List<InputController>();

        // Init Players
        _players = new List<PlayerController>();
        playerControllerPool.Prewarm(initialSize);
    }

    private void OnEnable() {
        inputControllerInstancedChannel.OnEventRaised += InputControllerInstanced;
        inputControllerDestroyedChannel.OnEventRaised += InputControllerDestroyed;
        spawnPlayerControllerChannel.OnEventRaised += SpawnPlayer;
        despawnPlayerControllerChannel.OnEventRaised += DespawnPlayer;
        setPlayerControllerParentChannel.OnEventRaised += SetPlayersParent;
    }

    private void OnDisable() {
        inputControllerInstancedChannel.OnEventRaised -= InputControllerInstanced;
        inputControllerDestroyedChannel.OnEventRaised -= InputControllerDestroyed;
        spawnPlayerControllerChannel.OnEventRaised -= SpawnPlayer;
        despawnPlayerControllerChannel.OnEventRaised -= DespawnPlayer;
        setPlayerControllerParentChannel.OnEventRaised -= SetPlayersParent;
    }

    private void InputControllerInstanced(GameObject go) {
        go.transform.SetParent(this.transform);
        _inputs.Add(go.GetComponent<InputController>());
    }
    
    private void InputControllerDestroyed(GameObject go) {
        var inputController = go.GetComponent<InputController>();
        var playerController = inputController.GetPlayerController();
        
        if (playerController != null) {
            DespawnPlayer(playerController);
        }
        _inputs.Remove(inputController);
    }
	private void SetPlayersParent(Transform parent) {
		playerControllerPool.SetParent(parent);
		
		foreach (var player in _players) {
			player.transform.SetParent(parent);
		}
	}
	
	private void SpawnPlayer() {
        // Find the first InputController that is not in use
        foreach (var inputController in _inputs) {
            if (inputController.GetPlayerController() == null) {
                
                var playerController = playerControllerPool.Request();
		        _players.Add(playerController);
                
                inputController.SetPlayerController(playerController);
                playerController.SetPlayerInput(inputController);
                return;
            }
        } 
        
        Debug.LogError("InputControllerManager: Cannot spawn more players, no more input controllers available. Connect another Device.");
	}
	
	private void DespawnPlayer(PlayerController playerController) {
		_players.Remove(playerController);
		playerControllerPool.Return(playerController);
	}
}