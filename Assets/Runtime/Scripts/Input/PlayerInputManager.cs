using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.Rendering;

public class PlayerInputManager : MonoBehaviour {
	
	[Header("PlayerInput Pool")]
	[SerializeField] private PlayerInputPoolSO pool;
	[SerializeField] private int initialSize = 1;
	
	[Header("Listening on Channels")]
	[Tooltip("Adds a PlayerInputHandler to the list of active players")]
	[SerializeField] private GameObjectEventChannelSO _AddPlayerInputChannel = default;
	[Tooltip("Removes a PlayerInputHandler from the list of active players")]
	[SerializeField] private GameObjectEventChannelSO _RemovePlayerInputChannel = default;
	
	private List<PlayerInputHandler> _playerInputs;
	private InputUser _inputUser;
	
	private void Awake() {
		_playerInputs = new List<PlayerInputHandler>();

		pool.Prewarm(initialSize);
		pool.SetParent(this.transform);
	}

	private void Update() {
	}
	
	private void OnEnable() {
		_AddPlayerInputChannel.OnEventRaised += AddPlayerInput;
		_RemovePlayerInputChannel.OnEventRaised += RemovePlayerInput;
	}

	private void OnDisable() {
		_AddPlayerInputChannel.OnEventRaised -= AddPlayerInput;
		_RemovePlayerInputChannel.OnEventRaised -= RemovePlayerInput;
	}

	private void AddPlayerInput(GameObject go) {
		PlayerController playerController = null;
		if (!ValidatePlayerGameObject(go, ref playerController, "AddPlayerInputChannel")) { return; }
		
		PlayerInputHandler playerInputHandler = pool.Request();
		playerController.SetPlayerInput(playerInputHandler);
		_playerInputs.Add(playerInputHandler);
	}

	private void RemovePlayerInput(GameObject go) {
		PlayerController playerController = null;
		if (!ValidatePlayerGameObject(go, ref playerController, "RemovePlayerInputChannel")) { return; }
		
		PlayerInputHandler playerInputHandler = playerController.GetPlayerInput();
		_playerInputs.Remove(playerInputHandler);
		pool.Return(playerInputHandler);
	}

	/// <summary>
	/// Checks if the GameObject is valid and has a PlayerController component.
	/// </summary>
	/// <param name="gameObject"> The GameObject to check. </param>
	/// <param name="playerControllerRef"> Pass in a reference to a PlayerController. If the GameObject is valid, this will be set to the PlayerController component on the GameObject. </param>
	/// <param name="channelName"> The name of the channel that is calling this method. Used for logging. </param>
	/// <returns></returns>
	private bool ValidatePlayerGameObject(GameObject gameObject, ref PlayerController playerControllerRef, string channelName) {
		if (gameObject == null) { 
			Debug.LogError(channelName + " received a null GameObject");
			return false;
		}
		
		var playerController = gameObject.GetComponent<PlayerController>();
		if (playerController == null) {
			Debug.LogError(channelName + " received a GameObject without a PlayerController");
			return false;
		}
		
		playerControllerRef = playerController;
		return true;
	}
}
