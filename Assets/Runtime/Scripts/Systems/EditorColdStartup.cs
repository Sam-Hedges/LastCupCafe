using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

/// <summary>
/// Allows a "cold start" in the editor, when pressing Play and not passing from the Initialisation scene.
/// </summary> 
public class EditorColdStartup : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField] private GameSceneSO _thisSceneSO = default;
	[SerializeField] private GameSceneSO _persistentManagersSO = default;
	[SerializeField] private AssetReference _notifyColdStartupChannel = default;
	[SerializeField] private VoidEventChannelSO _onSceneReadyChannel = default;
	[SerializeField] private WorkstationStateEventChannelSO _workstationStateUpdateChannel;
	[SerializeField] private GameObjectEventChannelSO _iconCanvasReadyChannel;

	private List<Workstation> _uninitialisedWorkstationIcons;
	private bool isColdStart = false;
	private void Awake()
	{
		if (!SceneManager.GetSceneByName(_persistentManagersSO.sceneReference.editorAsset.name).isLoaded)
		{
			isColdStart = true;
		}

		_workstationStateUpdateChannel.OnEventRaised += AddWorkstationIcon;
		_iconCanvasReadyChannel.OnEventRaised += InitialiseGameplayIconManager;
		_uninitialisedWorkstationIcons = new List<Workstation>();
	}

	private void AddWorkstationIcon(Workstation workstation) {
		_uninitialisedWorkstationIcons.Add(workstation);
	}

	private void InitialiseGameplayIconManager(GameObject go) {
		if (go.TryGetComponent(out GameplayIconManager manager)) {
			foreach (var workstation in _uninitialisedWorkstationIcons) {
				manager.UpdateIcon(gameObject);
			}
			_uninitialisedWorkstationIcons.Clear();
		}
	}

	private void Start()
	{
		if (isColdStart)
		{
			_persistentManagersSO.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += LoadEventChannel;
		}
	}
	private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj)
	{
		_notifyColdStartupChannel.LoadAssetAsync<LoadEventChannelSO>().Completed += OnNotifyChannelLoaded;
	}

	private void OnNotifyChannelLoaded(AsyncOperationHandle<LoadEventChannelSO> obj)
	{
		if (_thisSceneSO != null)
		{
			obj.Result.RaiseEvent(_thisSceneSO);
		}
		else
		{
			//Raise a fake scene ready event, so the player is spawned
			_onSceneReadyChannel.RaiseEvent();
			//When this happens, the player won't be able to move between scenes because the SceneLoader has no conception of which scene we are in
		}
	}

	private void OnDestroy() {
		_workstationStateUpdateChannel.OnEventRaised -= AddWorkstationIcon;
		_iconCanvasReadyChannel.OnEventRaised -= InitialiseGameplayIconManager;
	}

	#endif
}
