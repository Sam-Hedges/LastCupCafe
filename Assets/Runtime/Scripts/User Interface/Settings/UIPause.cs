using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
	[SerializeField] private InputHandler _inputReader = default;
	[SerializeField] private Button _resumeButton = default;
	[SerializeField] private Button _settingsButton = default;
	[SerializeField] private Button _backToMenuButton = default;

	[Header("Listening to")]
	[SerializeField] private BoolEventChannelSO _onPauseOpened = default;

	public event UnityAction Resumed = default;
	public event UnityAction SettingsScreenOpened = default;
	public event UnityAction BackToMainRequested = default;

	private void OnEnable()
	{
		_onPauseOpened.RaiseEvent(true);

		_resumeButton.Select();
		_inputReader.MenuCloseEvent += Resume;
		_resumeButton.onClick.AddListener(Resume);
		_settingsButton.onClick.AddListener(OpenSettingsScreen);
		_backToMenuButton.onClick.AddListener(BackToMainMenuConfirmation);
	}

	private void OnDisable()
	{
		_onPauseOpened.RaiseEvent(false);
		
		_inputReader.MenuCloseEvent -= Resume;
		_resumeButton.onClick.AddListener(Resume);
		_settingsButton.onClick.AddListener(OpenSettingsScreen);
		_backToMenuButton.onClick.AddListener(BackToMainMenuConfirmation);
	}

	void Resume()
	{
		Resumed.Invoke();
	}

	void OpenSettingsScreen()
	{
		SettingsScreenOpened.Invoke();
	}

	void BackToMainMenuConfirmation()
	{
		BackToMainRequested.Invoke();
	}

	public void CloseScreen()
	{
		Resumed.Invoke();
	}
}
