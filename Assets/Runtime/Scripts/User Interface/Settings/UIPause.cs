using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
	[SerializeField] private InputHandler inputReader = default;
	[SerializeField] private Button resumeButton = default;
	[SerializeField] private Button settingsButton = default;
	[SerializeField] private Button backToMenuButton = default;

	[Header("Listening to")]
	[SerializeField] private BoolEventChannelSO onPauseOpened = default;

	public event UnityAction Resumed = default;
	public event UnityAction SettingsScreenOpened = default;
	public event UnityAction BackToMainRequested = default;

	private void OnEnable()
	{
		onPauseOpened.RaiseEvent(true);

		resumeButton.Select();
		inputReader.MenuCloseEvent += Resume;
		resumeButton.onClick.AddListener(Resume);
		settingsButton.onClick.AddListener(OpenSettingsScreen);
		backToMenuButton.onClick.AddListener(BackToMainMenuConfirmation);
	}

	private void OnDisable()
	{
		onPauseOpened.RaiseEvent(false);
		
		inputReader.MenuCloseEvent -= Resume;
		resumeButton.onClick.AddListener(Resume);
		settingsButton.onClick.AddListener(OpenSettingsScreen);
		backToMenuButton.onClick.AddListener(BackToMainMenuConfirmation);
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
