using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnSystem : MonoBehaviour
{
	[Header("Asset References")]
	[SerializeField] private InputHandler inputHandler = default;
	[SerializeField] private readonly GameObject playerPrefab = default;
	[SerializeField] private TransformAnchor _playerTransformAnchor = default;
	[SerializeField] private TransformEventChannelSO _playerInstantiatedChannel = default;

	[Header("Scene Ready Event")]
	[SerializeField] private VoidEventChannelSO _onSceneReady = default; //Raised by SceneLoader when the scene is set to active
	
	private Transform _defaultSpawnPoint;

	private void Awake()
	{
		_defaultSpawnPoint = transform.GetChild(0);
	}

	private void OnEnable()
	{
		_onSceneReady.OnEventRaised += SpawnPlayer;
	}

	private void OnDisable()
	{
		_onSceneReady.OnEventRaised -= SpawnPlayer;

		_playerTransformAnchor.Unset();
	}

	private void SpawnPlayer()
	{
		Transform spawnLocation = _defaultSpawnPoint;
		GameObject playerInstance = Instantiate(playerPrefab, spawnLocation.position, spawnLocation.rotation);

		_playerInstantiatedChannel.RaiseEvent(playerInstance.transform);
		_playerTransformAnchor.Provide(playerInstance.transform); //the CameraSystem will pick this up to frame the player

		// TODO: Probably move this to the GameManager once it's up and running
		inputHandler.EnableGameplayInput();
	}
}
