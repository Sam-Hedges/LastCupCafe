using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private GameStateSO _gameState = default;
	
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
	
	private void Start()
	{
		StartGame();
	}
	
	void StartGame()
	{
		_gameState.UpdateGameState(GameState.Gameplay);
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
		
		inputHandler.EnableGameplayInput();
	}
}
