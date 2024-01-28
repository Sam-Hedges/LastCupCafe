using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


/// <summary>
/// This class contains the function to call when play button is pressed
/// </summary>
public class StartGame : MonoBehaviour
{
	[SerializeField] private GameSceneSO locationsToLoad;
	
	[Header("Broadcasting on")]
	[SerializeField] private LoadEventChannelSO loadLocation = default;

	[Header("Listening to")]
	[SerializeField] private VoidEventChannelSO onNewGameButton = default;

	private void Start()
	{
		onNewGameButton.OnEventRaised += StartNewGame;
	}

	private void OnDestroy()
	{
		onNewGameButton.OnEventRaised -= StartNewGame;
	}

	private void StartNewGame()
	{
		loadLocation.RaiseEvent(locationsToLoad);
	}
	
}
