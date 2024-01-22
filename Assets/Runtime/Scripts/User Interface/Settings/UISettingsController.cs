using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public enum SettingFieldType
{
	Volume_SFx,
	Volume_Music,
	Resolution,
	FullScreen,
	ShadowDistance,
	AntiAliasing,
	ShadowQuality,
	Volume_Master,

}
[System.Serializable]
public class SettingTab
{
	public SettingsType settingTabsType;
}

[System.Serializable]
public class SettingField
{
	public SettingsType settingTabsType;
	public SettingFieldType settingFieldType;
	public string title;
}

public enum SettingsType
{
	Language,
	Graphics,
	Audio,
}
public class UISettingsController : MonoBehaviour
{
	[SerializeField] private UISettingsGraphicsComponent _graphicsComponent;
	[SerializeField] private UISettingsAudioComponent _audioComponent;
	[SerializeField] private SettingsSO _currentSettings = default;
	[SerializeField] private List<SettingsType> _settingTabsList = new List<SettingsType>();
	private SettingsType _selectedTab = SettingsType.Audio;
	[SerializeField] private InputHandler _inputReader = default;
	[SerializeField] private VoidEventChannelSO SaveSettingsEvent = default;
	public UnityAction Closed;
	private void OnEnable()
	{
		_audioComponent._save += SaveAudioSettings;
		_graphicsComponent._save += SaveGraphicsSettings;

		//_inputReader.MenuCloseEvent += CloseScreen;
		//_inputReader.TabSwitched += SwitchTab;

		OpenSetting(SettingsType.Audio);

	}
	private void OnDisable()
	{
		//_inputReader.MenuCloseEvent -= CloseScreen;
		//_inputReader.TabSwitched -= SwitchTab;

		_audioComponent._save -= SaveAudioSettings;
		_graphicsComponent._save -= SaveGraphicsSettings;
	}
	public void CloseScreen()
	{
		Closed.Invoke();
	}



	void OpenSetting(SettingsType settingType)
	{
		_selectedTab = settingType;
		switch (settingType)
		{
			case SettingsType.Graphics:
				_graphicsComponent.Setup();
				break;
			case SettingsType.Audio:
				_audioComponent.Setup(_currentSettings.MusicVolume, _currentSettings.SfxVolume, _currentSettings.MasterVolume);
				break;
			default:
				break;
		}

		_graphicsComponent.gameObject.SetActive((settingType == SettingsType.Graphics));
		_audioComponent.gameObject.SetActive(settingType == SettingsType.Audio);

	}
	void SwitchTab(float orientation)
	{

		if (orientation != 0)
		{
			bool isLeft = orientation < 0;
			int initialIndex = _settingTabsList.FindIndex(o => o == _selectedTab);
			if (initialIndex != -1)
			{
				if (isLeft)
				{
					initialIndex--;
				}
				else
				{
					initialIndex++;
				}

				initialIndex = Mathf.Clamp(initialIndex, 0, _settingTabsList.Count - 1);
			}

			OpenSetting(_settingTabsList[initialIndex]);
		}
	}
	public void SaveGraphicsSettings(int newResolutionsIndex, int newAntiAliasingIndex, float newShadowDistance, bool fullscreenState)
	{
		_currentSettings.SaveGraphicsSettings(newResolutionsIndex, newAntiAliasingIndex, newShadowDistance, fullscreenState);
		SaveSettingsEvent.RaiseEvent();
	}
	void SaveAudioSettings(float musicVolume, float sfxVolume, float masterVolume)
	{
		_currentSettings.SaveAudioSettings(musicVolume, sfxVolume, masterVolume);

		SaveSettingsEvent.RaiseEvent();
	}

}
