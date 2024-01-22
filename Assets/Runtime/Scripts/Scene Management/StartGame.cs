using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


/// <summary>
/// This class contains the function to call when play button is pressed
/// </summary>
public class StartGame : MonoBehaviour
{
	[SerializeField] private GameSceneSO _locationsToLoad;
	[SerializeField] private bool _showLoadScreen = default;
	
	[Header("Broadcasting on")]
	[SerializeField] private LoadEventChannelSO _loadLocation = default;

	[Header("Listening to")]
	[SerializeField] private VoidEventChannelSO _onNewGameButton = default;

	private bool _hasSaveData;

	private void Start()
	{
		_onNewGameButton.OnEventRaised += StartNewGame;
	}

	private void OnDestroy()
	{
		_onNewGameButton.OnEventRaised -= StartNewGame;
	}

	private void StartNewGame()
	{
		_hasSaveData = false;
		
		_loadLocation.RaiseEvent(_locationsToLoad);
	}
	

	private void OnResetSaveDataPress()
	{
		_hasSaveData = false;
	}

	
}
