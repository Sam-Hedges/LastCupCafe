using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISettingsAudioComponent : MonoBehaviour
{
	[Header("Sliders")]
	[SerializeField] private Slider musicVolumeSlider = default;
	[SerializeField] private Slider sfxVolumeSlider = default;
	[SerializeField] private Slider masterVolumeSlider = default;
	
	[Header("Broadcasting")]
	[SerializeField] private FloatEventChannelSO masterVolumeEventChannel = default;
	[SerializeField] private FloatEventChannelSO sFXVolumeEventChannel = default;
	[SerializeField] private FloatEventChannelSO musicVolumeEventChannel = default;
	private float MusicVolume { get; set; }
	private float SfxVolume { get; set; }
	private float MasterVolume { get; set; }
	private float SavedMusicVolume { get; set; }
	private float SavedSfxVolume { get; set; }
	private float SavedMasterVolume { get; set; }

	public event UnityAction<float, float, float> Save = delegate { };
	
	private void OnDisable()
	{
		ResetVolumes(); // reset volumes on disable. If not saved, it will reset to initial volumes. 
	}
	public void Setup(float musicVolume, float sfxVolume, float masterVolume)
	{
		MasterVolume = masterVolume;
		MusicVolume = musicVolume;
		SfxVolume = sfxVolume;
		
		masterVolumeSlider.value = MasterVolume * 10;
		musicVolumeSlider.value = MasterVolume * 10;
		sfxVolumeSlider.value = SfxVolume * 10;
		
		SavedMasterVolume = MasterVolume;
		SavedMusicVolume = MusicVolume;
		SavedSfxVolume = SfxVolume;

		SetMusicVolume();
		SetSfxVolume();
		SetMasterVolume();
	}
	
	private float ReturnSliderValue(Slider slider)
	{
		float value = slider.value;
		
		if (value < 0 || value > 11)
		{
			Debug.LogError("Slider value is out of range");
			return 0;
		}

		return value;
	}
	
	public void SetMusicVolumeField(Slider slider)
	{
		MusicVolume = ReturnSliderValue(slider) / 10;
		
		SetMusicVolume();
	}

	public void SetSfxVolumeField(Slider slider)
	{
		SfxVolume = ReturnSliderValue(slider) / 10;
		
		SetSfxVolume();
	}

	public void SetMasterVolumeField(Slider slider)
	{
		MasterVolume = ReturnSliderValue(slider) / 10;
		
		SetMasterVolume();
	}
	private void SetMusicVolume()
	{
		musicVolumeEventChannel.RaiseEvent(MusicVolume);//raise event for volume change
		SaveVolumes();
	}
	private void SetSfxVolume()
	{
		sFXVolumeEventChannel.RaiseEvent(SfxVolume); //raise event for volume change
		SaveVolumes();
	}
	private void SetMasterVolume()
	{
		masterVolumeEventChannel.RaiseEvent(MasterVolume); //raise event for volume change
		SaveVolumes();
	}

	public void ResetVolumes()
	{
		Setup(1, 1, 1);
	}
	private void SaveVolumes()
	{
		SavedMasterVolume = MasterVolume;
		SavedMusicVolume = MusicVolume;
		SavedSfxVolume = SfxVolume;
		//save Audio
		Save.Invoke(MusicVolume, SfxVolume, MasterVolume);
	}


}
