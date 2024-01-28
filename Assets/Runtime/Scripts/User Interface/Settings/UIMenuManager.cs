using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuManager : MonoBehaviour
{
	[SerializeField] private UISettingsController settingsPanel = default;
	[SerializeField] private UICredits creditsPanel = default;
	[SerializeField] private UIMainMenu mainMenuPanel = default;

	[SerializeField] private SaveSystem saveSystem = default;

	[SerializeField] private InputHandler inputReader = default;


	[Header("Broadcasting on")]
	[SerializeField]
	private VoidEventChannelSO startNewGameEvent = default;
	[SerializeField]
	private VoidEventChannelSO continueGameEvent = default;



	private bool _hasSaveData;

	private IEnumerator Start()
	{
		inputReader.EnableMenuInput();
		yield return new WaitForSeconds(0.4f); //waiting time for all scenes to be loaded 
		SetMenuScreen();
	}
	void SetMenuScreen()
	{
		_hasSaveData = saveSystem.LoadSaveDataFromDisk();
		mainMenuPanel.SetMenuScreen(_hasSaveData);
		mainMenuPanel.ContinueButtonAction += continueGameEvent.RaiseEvent;
		mainMenuPanel.SettingsButtonAction += OpenSettingsScreen;
		mainMenuPanel.CreditsButtonAction += OpenCreditsScreen;
	}

	void ConfirmStartNewGame()
	{
		startNewGameEvent.RaiseEvent();
	}

	public void OpenSettingsScreen()
	{
		settingsPanel.gameObject.SetActive(true);
		settingsPanel.Closed += CloseSettingsScreen;

	}
	public void CloseSettingsScreen()
	{
		settingsPanel.Closed -= CloseSettingsScreen;
		settingsPanel.gameObject.SetActive(false);
		mainMenuPanel.SetMenuScreen(_hasSaveData);

	}
	public void OpenCreditsScreen()
	{
		creditsPanel.gameObject.SetActive(true);

		creditsPanel.OnCloseCredits += CloseCreditsScreen;

	}
	public void CloseCreditsScreen()
	{
		creditsPanel.OnCloseCredits -= CloseCreditsScreen;
		creditsPanel.gameObject.SetActive(false);
		mainMenuPanel.SetMenuScreen(_hasSaveData);

	}
}
