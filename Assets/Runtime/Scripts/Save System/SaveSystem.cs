using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "Save System/Save System")]
public class SaveSystem : ScriptableObject
{
	[SerializeField] private VoidEventChannelSO _saveSettingsEvent = default;
	[SerializeField] private LoadEventChannelSO _loadLevel = default;
	[SerializeField] private SettingsSO _currentSettings = default;

	public string saveFilename = "save.eclipse";
	public string backupSaveFilename = "save.eclipse.bak";
	public Save saveData = new Save();

	void OnEnable()
	{
		_saveSettingsEvent.OnEventRaised += SaveSettings;
		_loadLevel.OnLoadingRequested += CacheLoadLevels;
	}

	void OnDisable()
	{
		_saveSettingsEvent.OnEventRaised -= SaveSettings;
		_loadLevel.OnLoadingRequested -= CacheLoadLevels;
	}

	private void CacheLoadLevels(GameSceneSO levelToLoad, bool showLoadingScreen, bool fadeScreen)
	{
		 LevelSO levelSO = levelToLoad as LevelSO;
		if (levelSO)
		{
			saveData._levelId = levelSO.Guid;
		}

		SaveDataToDisk();
	}

	public bool LoadSaveDataFromDisk()
	{
		if (FileManager.LoadFromFile(saveFilename, out var json))
		{
			saveData.LoadFromJson(json);
			return true;
		}

		return false;
	}

	public void SaveDataToDisk()
	{
		if (FileManager.MoveFile(saveFilename, backupSaveFilename))
		{
			if (FileManager.WriteToFile(saveFilename, saveData.ToJson()))
			{
				//Debug.Log("Save successful " + saveFilename);
			}
		}
	}

	public void WriteEmptySaveFile()
	{
		FileManager.WriteToFile(saveFilename, "");

	}
	public void SetNewGameData()
	{
		FileManager.WriteToFile(saveFilename, "");

		SaveDataToDisk();

	}
	void SaveSettings()
	{
		saveData.SaveSettings(_currentSettings);

	}
}
