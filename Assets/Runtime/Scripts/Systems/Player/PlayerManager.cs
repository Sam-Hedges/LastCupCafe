using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    
	[Header("PlayerInput Pool")]
	[SerializeField] private PlayerControllerPoolSO pool;
	[SerializeField] private int initialSize = 1;
	
	
	[Header("Listening on Channels")]
	[Tooltip("Spawns a Player at the spawn point")]
	[SerializeField] private VoidEventChannelSO _SpawnPlayerChannel = default;
	[Tooltip("Removes a Player from the scene")]
	[SerializeField] private GameObjectEventChannelSO _DespawnPlayerChannel = default;
	[Tooltip("Sets the Player's parent to the PlayerParent object")]
	[SerializeField] private TransformEventChannelSO _PlayerParentChannel = default;
	
	
	[Header("Broadcasting on Channels")]
	[Tooltip("Adds a PlayerInputHandler to the list of active players")]
	[SerializeField] private GameObjectEventChannelSO _AddPlayerInputChannel = default;
	[Tooltip("Removes a PlayerInputHandler from the list of active players")]
	[SerializeField] private GameObjectEventChannelSO _RemovePlayerInputChannel = default;
	
	private List<GameObject> _players;
	
	private void Awake() {
		_players = new List<GameObject>();

		pool.Prewarm(initialSize);
	}
	
	private void OnEnable() {
		_SpawnPlayerChannel.OnEventRaised += SpawnPlayer;
		_DespawnPlayerChannel.OnEventRaised += DespawnPlayer;
		_PlayerParentChannel.OnEventRaised += SetPlayerParent;
	}

	private void OnDisable() {
		_SpawnPlayerChannel.OnEventRaised -= SpawnPlayer;
		_DespawnPlayerChannel.OnEventRaised -= DespawnPlayer;
		_PlayerParentChannel.OnEventRaised -= SetPlayerParent;
	}
	
	private void SetPlayerParent(Transform parent) {
		pool.SetParent(parent);
		
		foreach (var player in _players) {
			player.transform.SetParent(parent);
		}
	}
	
	private void SpawnPlayer() {
		GameObject player = pool.Request().gameObject;
		_AddPlayerInputChannel.RaiseEvent(player);
		_players.Add(player);
	}
	
	private void DespawnPlayer(GameObject go) {
		PlayerController playerController = null;
		if (!ValidatePlayerGameObject(go, ref playerController, "DespawnPlayerChannel")) { return; }
		
		_RemovePlayerInputChannel.RaiseEvent(go);
		_players.Remove(go);
		pool.Return(playerController);
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