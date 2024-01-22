using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
	[Header("Scene UI")]
	[SerializeField] private MenuSelectionHandler _selectionHandler = default;
	[SerializeField] private GameObject _switchTabDisplay = default;
	[SerializeField] private UIPause _pauseScreen = default;
	[SerializeField] private UISettingsController _settingScreen = default;

	[Header("Gameplay")]
	[SerializeField] private GameStateSO _gameStateManager = default;
	[SerializeField] private MenuSO _mainMenu = default;
	[SerializeField] private InputHandler _inputReader = default;

	[Header("Listening on")]
	[SerializeField] private VoidEventChannelSO _onSceneReady = default;

	[Header("Broadcasting on ")]
	[SerializeField] private LoadEventChannelSO _loadMenuEvent = default;
	[SerializeField] private VoidEventChannelSO _onInteractionEndedEvent = default;

	bool isForCooking = false;

	private void OnEnable()
	{
		_onSceneReady.OnEventRaised += ResetUI;
		_inputReader.MenuPauseEvent += OpenUIPause; // subscription to open Pause UI event happens in OnEnabled, but the close Event is only subscribed to when the popup is open
	}

	private void OnDisable()
	{
		_onSceneReady.OnEventRaised -= ResetUI;
		_inputReader.MenuPauseEvent -= OpenUIPause;
	}

	void ResetUI()
	{
		_pauseScreen.gameObject.SetActive(false);
		_switchTabDisplay.SetActive(false);

		Time.timeScale = 1;
	}

	void OpenUIPause()
	{
		_inputReader.MenuPauseEvent -= OpenUIPause; // you can open UI pause menu again, if it's closed

		Time.timeScale = 0; // Pause time

		_pauseScreen.SettingsScreenOpened += OpenSettingScreen;//once the UI Pause popup is open, listen to open Settings 
		_pauseScreen.BackToMainRequested += ShowBackToMenuConfirmationPopup;//once the UI Pause popup is open, listen to back to menu button
		_pauseScreen.Resumed += CloseUIPause;//once the UI Pause popup is open, listen to unpause event

		_pauseScreen.gameObject.SetActive(true);

		_inputReader.EnableMenuInput();
		_gameStateManager.UpdateGameState(GameState.Pause);
	}

	void CloseUIPause()
	{
		Time.timeScale = 1; // unpause time

		_inputReader.MenuPauseEvent += OpenUIPause; // you can open UI pause menu again, if it's closed

		// once the popup is closed, you can't listen to the following events 
		_pauseScreen.SettingsScreenOpened -= OpenSettingScreen;//once the UI Pause popup is open, listen to open Settings 
		_pauseScreen.BackToMainRequested -= ShowBackToMenuConfirmationPopup;//once the UI Pause popup is open, listen to back to menu button
		_pauseScreen.Resumed -= CloseUIPause;//once the UI Pause popup is open, listen to unpause event

		_pauseScreen.gameObject.SetActive(false);

		_gameStateManager.ResetToPreviousGameState();
		
		if (_gameStateManager.CurrentGameState == GameState.Gameplay)
		{
			_inputReader.EnableGameplayInput();
		}

		_selectionHandler.Unselect();
	}

	void OpenSettingScreen()
	{
		_settingScreen.Closed += CloseSettingScreen; // sub to close setting event with event 

		_pauseScreen.gameObject.SetActive(false); // Set pause screen to inactive

		_settingScreen.gameObject.SetActive(true);// set Setting screen to active 

		// time is still set to 0 and Input is still set to menuInput 
	}

	void CloseSettingScreen()
	{
		//unsub from close setting events 
		_settingScreen.Closed -= CloseSettingScreen;

		_selectionHandler.Unselect();
		_pauseScreen.gameObject.SetActive(true); // Set pause screen to inactive

		_settingScreen.gameObject.SetActive(false);

		// time is still set to 0 and Input is still set to menuInput 
		//going out from setting screen gets us back to the pause screen
	}

	void ShowBackToMenuConfirmationPopup()
	{
		_pauseScreen.gameObject.SetActive(false); // Set pause screen to inactive

		_inputReader.EnableMenuInput();
	}

	void BackToMainMenu(bool confirm)
	{
		HideBackToMenuConfirmationPopup();// hide confirmation screen, show close UI pause, 

		if (confirm)
		{
			CloseUIPause();//close ui pause to unsub from all events 
			_loadMenuEvent.RaiseEvent(_mainMenu); //load main menu
		}
	}
	
	void HideBackToMenuConfirmationPopup()
	{
		_selectionHandler.Unselect();
		_pauseScreen.gameObject.SetActive(true); // Set pause screen to inactive

		// time is still set to 0 and Input is still set to menuInput 
		//going out from confirmaiton popup screen gets us back to the pause screen
	}
}