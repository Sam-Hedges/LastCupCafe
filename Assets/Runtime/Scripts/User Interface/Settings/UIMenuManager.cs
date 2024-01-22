using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuManager : MonoBehaviour
{
	[SerializeField] private UISettingsController _settingsPanel = default;
	[SerializeField] private UICredits _creditsPanel = default;
	[SerializeField] private UIMainMenu _mainMenuPanel = default;

	[SerializeField] private SaveSystem _saveSystem = default;

	[SerializeField] private InputHandler _inputReader = default;


	[Header("Broadcasting on")]
	[SerializeField]
	private VoidEventChannelSO _startNewGameEvent = default;
	[SerializeField]
	private VoidEventChannelSO _continueGameEvent = default;



	private bool _hasSaveData;

	private IEnumerator Start()
	{
		_inputReader.EnableMenuInput();
		yield return new WaitForSeconds(0.4f); //waiting time for all scenes to be loaded 
		SetMenuScreen();
	}
	void SetMenuScreen()
	{
		_hasSaveData = _saveSystem.LoadSaveDataFromDisk();
		_mainMenuPanel.SetMenuScreen(_hasSaveData);
		_mainMenuPanel.ContinueButtonAction += _continueGameEvent.RaiseEvent;
		_mainMenuPanel.SettingsButtonAction += OpenSettingsScreen;
		_mainMenuPanel.CreditsButtonAction += OpenCreditsScreen;
	}

	void ConfirmStartNewGame()
	{
		_startNewGameEvent.RaiseEvent();
	}

	public void OpenSettingsScreen()
	{
		_settingsPanel.gameObject.SetActive(true);
		_settingsPanel.Closed += CloseSettingsScreen;

	}
	public void CloseSettingsScreen()
	{
		_settingsPanel.Closed -= CloseSettingsScreen;
		_settingsPanel.gameObject.SetActive(false);
		_mainMenuPanel.SetMenuScreen(_hasSaveData);

	}
	public void OpenCreditsScreen()
	{
		_creditsPanel.gameObject.SetActive(true);

		_creditsPanel.OnCloseCredits += CloseCreditsScreen;

	}
	public void CloseCreditsScreen()
	{
		_creditsPanel.OnCloseCredits -= CloseCreditsScreen;
		_creditsPanel.gameObject.SetActive(false);
		_mainMenuPanel.SetMenuScreen(_hasSaveData);

	}
}
