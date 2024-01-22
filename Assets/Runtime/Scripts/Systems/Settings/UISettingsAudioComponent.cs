using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISettingsAudioComponent : MonoBehaviour
{
	[Header("Sliders")]
	[SerializeField] private Slider _musicVolumeSlider = default;
	[SerializeField] private Slider _sfxVolumeSlider = default;
	[SerializeField] private Slider _masterVolumeSlider = default;
	
	[Header("Broadcasting")]
	[SerializeField] private FloatEventChannelSO _masterVolumeEventChannel = default;
	[SerializeField] private FloatEventChannelSO _sFXVolumeEventChannel = default;
	[SerializeField] private FloatEventChannelSO _musicVolumeEventChannel = default;
	private float _musicVolume { get; set; }
	private float _sfxVolume { get; set; }
	private float _masterVolume { get; set; }
	private float _savedMusicVolume { get; set; }
	private float _savedSfxVolume { get; set; }
	private float _savedMasterVolume { get; set; }

	int _maxVolume = 10;

	public event UnityAction<float, float, float> _save = delegate { };
	
	private void OnDisable()
	{
		ResetVolumes(); // reset volumes on disable. If not saved, it will reset to initial volumes. 
	}
	public void Setup(float musicVolume, float sfxVolume, float masterVolume)
	{
		_masterVolume = masterVolume;
		_musicVolume = musicVolume;
		_sfxVolume = sfxVolume;
		
		_masterVolumeSlider.value = _masterVolume * 10;
		_musicVolumeSlider.value = _masterVolume * 10;
		_sfxVolumeSlider.value = _sfxVolume * 10;
		
		_savedMasterVolume = _masterVolume;
		_savedMusicVolume = _musicVolume;
		_savedSfxVolume = _sfxVolume;

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
		_musicVolume = ReturnSliderValue(slider) / 10;
		
		SetMusicVolume();
	}

	public void SetSfxVolumeField(Slider slider)
	{
		_sfxVolume = ReturnSliderValue(slider) / 10;
		
		SetSfxVolume();
	}

	public void SetMasterVolumeField(Slider slider)
	{
		_masterVolume = ReturnSliderValue(slider) / 10;
		
		SetMasterVolume();
	}
	private void SetMusicVolume()
	{
		_musicVolumeEventChannel.RaiseEvent(_musicVolume);//raise event for volume change
		SaveVolumes();
	}
	private void SetSfxVolume()
	{
		_sFXVolumeEventChannel.RaiseEvent(_sfxVolume); //raise event for volume change
		SaveVolumes();
	}
	private void SetMasterVolume()
	{
		_masterVolumeEventChannel.RaiseEvent(_masterVolume); //raise event for volume change
		SaveVolumes();
	}

	public void ResetVolumes()
	{
		Setup(1, 1, 1);
	}
	private void SaveVolumes()
	{
		_savedMasterVolume = _masterVolume;
		_savedMusicVolume = _musicVolume;
		_savedSfxVolume = _sfxVolume;
		//save Audio
		_save.Invoke(_musicVolume, _sfxVolume, _masterVolume);
	}


}
