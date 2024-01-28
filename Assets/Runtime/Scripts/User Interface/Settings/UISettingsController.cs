using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public enum SettingFieldType
{
	VolumeSFx,
	VolumeMusic,
	Resolution,
	FullScreen,
	ShadowDistance,
	AntiAliasing,
	ShadowQuality,
	VolumeMaster,

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
	[SerializeField] private UISettingsGraphicsComponent graphicsComponent;
	[SerializeField] private UISettingsAudioComponent audioComponent;
	[SerializeField] private SettingsSO currentSettings = default;
	[SerializeField] private List<SettingsType> settingTabsList = new List<SettingsType>();
	private SettingsType _selectedTab = SettingsType.Audio;
	[SerializeField] private VoidEventChannelSO saveSettingsEvent = default;
	public UnityAction Closed;
	private void OnEnable()
	{
		audioComponent.Save += SaveAudioSettings;
		graphicsComponent._save += SaveGraphicsSettings;

		//_inputReader.MenuCloseEvent += CloseScreen;
		//_inputReader.TabSwitched += SwitchTab;

		OpenSetting(SettingsType.Audio);

	}
	private void OnDisable()
	{
		//_inputReader.MenuCloseEvent -= CloseScreen;
		//_inputReader.TabSwitched -= SwitchTab;

		audioComponent.Save -= SaveAudioSettings;
		graphicsComponent._save -= SaveGraphicsSettings;
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
				graphicsComponent.Setup();
				break;
			case SettingsType.Audio:
				audioComponent.Setup(currentSettings.MusicVolume, currentSettings.SfxVolume, currentSettings.MasterVolume);
				break;
			default:
				break;
		}

		graphicsComponent.gameObject.SetActive((settingType == SettingsType.Graphics));
		audioComponent.gameObject.SetActive(settingType == SettingsType.Audio);

	}
	void SwitchTab(float orientation)
	{

		if (orientation != 0)
		{
			bool isLeft = orientation < 0;
			int initialIndex = settingTabsList.FindIndex(o => o == _selectedTab);
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

				initialIndex = Mathf.Clamp(initialIndex, 0, settingTabsList.Count - 1);
			}

			OpenSetting(settingTabsList[initialIndex]);
		}
	}
	public void SaveGraphicsSettings(int newResolutionsIndex, int newAntiAliasingIndex, float newShadowDistance, bool fullscreenState)
	{
		currentSettings.SaveGraphicsSettings(newResolutionsIndex, newAntiAliasingIndex, newShadowDistance, fullscreenState);
		saveSettingsEvent.RaiseEvent();
	}
	void SaveAudioSettings(float musicVolume, float sfxVolume, float masterVolume)
	{
		currentSettings.SaveAudioSettings(musicVolume, sfxVolume, masterVolume);

		saveSettingsEvent.RaiseEvent();
	}

}
