using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attached to an audio source to enable control over it via the audio manager
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
	private AudioSource _audioSource;

	public event UnityAction<SoundEmitter> OnSoundFinishedPlaying;

	private void Awake()
	{
		_audioSource = this.GetComponent<AudioSource>();
		_audioSource.playOnAwake = false;
	}

	/// <summary>
	/// Instructs the AudioSource to play a single clip, with optional looping, in a position in 3D space.
	/// </summary>
	/// <param name="clip"></param>
	/// <param name="settings"></param>
	/// <param name="hasToLoop"></param>
	/// <param name="position"></param>
	public void PlayAudioClip(AudioClip clip, AudioConfigurationSO settings, bool hasToLoop, Vector3 position = default)
	{
		_audioSource.clip = clip;
		settings.ApplyTo(_audioSource);
		_audioSource.transform.position = position;
		_audioSource.loop = hasToLoop;
		_audioSource.time = 0f; //Reset in case this AudioSource is being reused for a short SFX after being used for a long music track
		_audioSource.Play();

		if (!hasToLoop)
		{
			StartCoroutine(FinishedPlaying(clip.length));
		}
	}

	/// <summary>
	/// Fades in a music track, optionally starting it at a specific time
	/// </summary>
	/// <param name="musicClip"></param>
	/// <param name="settings"></param>
	/// <param name="duration"></param>
	/// <param name="startTime"></param>
	public void FadeMusicIn(AudioClip musicClip, AudioConfigurationSO settings, float duration, float startTime = 0f)
	{
		PlayAudioClip(musicClip, settings, true);
		_audioSource.volume = 0f;

		//Start the clip at the same time the previous one left, if length allows
		if (startTime <= _audioSource.clip.length)
			_audioSource.time = startTime;

		_audioSource.DOFade(settings.Volume, duration);
	}

	/// <summary>
	/// Fades out the music track currently playing
	/// </summary>
	/// <param name="duration"></param>
	/// <returns></returns>
	public float FadeMusicOut(float duration)
	{
		_audioSource.DOFade(0f, duration).onComplete += OnFadeOutComplete;

		return _audioSource.time;
	}

	private void OnFadeOutComplete()
	{
		NotifyBeingDone();
	}

	/// <summary>
	/// Used to check which music track is being played.
	/// </summary>
	public AudioClip GetCurrentAudioClip()
	{
		return _audioSource.clip;
	}


	/// <summary>
	/// Used when the game is unpaused, to pick up SFX from where they left.
	/// </summary>
	public void Resume()
	{
		_audioSource.Play();
	}

	/// <summary>
	/// Used when the game is paused.
	/// </summary>
	public void Pause()
	{
		_audioSource.Pause();
	}

	public void Stop()
	{
		_audioSource.Stop();
	}

	public void Finish()
	{
		if (_audioSource.loop)
		{
			_audioSource.loop = false;
			float timeRemaining = _audioSource.clip.length - _audioSource.time;
			StartCoroutine(FinishedPlaying(timeRemaining));
		}
	}

	public bool IsPlaying()
	{
		return _audioSource.isPlaying;
	}

	public bool IsLooping()
	{
		return _audioSource.loop;
	}

	IEnumerator FinishedPlaying(float clipLength)
	{
		yield return new WaitForSeconds(clipLength);

		NotifyBeingDone();
	}

	private void NotifyBeingDone()
	{
		OnSoundFinishedPlaying.Invoke(this); // The AudioManager will pick this up
	}
}
