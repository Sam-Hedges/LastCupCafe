using UnityEngine;

public class UIManager : MonoBehaviour
{
	[Header("Scene UI")]
	[SerializeField] private MenuSelectionHandler selectionHandler = default;
	[SerializeField] private GameObject switchTabDisplay = default;
	[SerializeField] private UIPause pauseScreen = default;
	[SerializeField] private UISettingsController settingScreen = default;

	[Header("Gameplay")]
	[SerializeField] private GameStateSO gameStateManager = default;
	[SerializeField] private MenuSO mainMenu = default;
	[SerializeField] private InputHandler inputReader = default;

	[Header("Listening on")]
	[SerializeField] private VoidEventChannelSO onSceneReady = default;

	[Header("Broadcasting on ")]
	[SerializeField] private LoadEventChannelSO loadMenuEvent = default;

	private void OnEnable()
	{
		onSceneReady.OnEventRaised += ResetUI;
		inputReader.MenuPauseEvent += OpenUIPause; // subscription to open Pause UI event happens in OnEnabled, but the close Event is only subscribed to when the popup is open
	}

	private void OnDisable()
	{
		onSceneReady.OnEventRaised -= ResetUI;
		inputReader.MenuPauseEvent -= OpenUIPause;
	}

	void ResetUI()
	{
		pauseScreen.gameObject.SetActive(false);
		switchTabDisplay.SetActive(false);

		Time.timeScale = 1;
	}

	void OpenUIPause()
	{
		inputReader.MenuPauseEvent -= OpenUIPause; // you can open UI pause menu again, if it's closed

		Time.timeScale = 0; // Pause time

		pauseScreen.SettingsScreenOpened += OpenSettingScreen;//once the UI Pause popup is open, listen to open Settings 
		pauseScreen.BackToMainRequested += ShowBackToMenuConfirmationPopup;//once the UI Pause popup is open, listen to back to menu button
		pauseScreen.Resumed += CloseUIPause;//once the UI Pause popup is open, listen to unpause event

		pauseScreen.gameObject.SetActive(true);

		inputReader.EnableMenuInput();
		gameStateManager.UpdateGameState(GameState.Pause);
	}

	void CloseUIPause()
	{
		Time.timeScale = 1; // unpause time

		inputReader.MenuPauseEvent += OpenUIPause; // you can open UI pause menu again, if it's closed

		// once the popup is closed, you can't listen to the following events 
		pauseScreen.SettingsScreenOpened -= OpenSettingScreen;//once the UI Pause popup is open, listen to open Settings 
		pauseScreen.BackToMainRequested -= ShowBackToMenuConfirmationPopup;//once the UI Pause popup is open, listen to back to menu button
		pauseScreen.Resumed -= CloseUIPause;//once the UI Pause popup is open, listen to unpause event

		pauseScreen.gameObject.SetActive(false);

		gameStateManager.ResetToPreviousGameState();
		
		if (gameStateManager.CurrentGameState == GameState.Gameplay)
		{
			inputReader.EnableGameplayInput();
		}

		selectionHandler.Unselect();
	}

	void OpenSettingScreen()
	{
		settingScreen.Closed += CloseSettingScreen; // sub to close setting event with event 

		pauseScreen.gameObject.SetActive(false); // Set pause screen to inactive

		settingScreen.gameObject.SetActive(true);// set Setting screen to active 

		// time is still set to 0 and Input is still set to menuInput 
	}

	void CloseSettingScreen()
	{
		//unsub from close setting events 
		settingScreen.Closed -= CloseSettingScreen;

		selectionHandler.Unselect();
		pauseScreen.gameObject.SetActive(true); // Set pause screen to inactive

		settingScreen.gameObject.SetActive(false);

		// time is still set to 0 and Input is still set to menuInput 
		//going out from setting screen gets us back to the pause screen
	}

	void ShowBackToMenuConfirmationPopup()
	{
		pauseScreen.gameObject.SetActive(false); // Set pause screen to inactive

		inputReader.EnableMenuInput();
	}

	void BackToMainMenu(bool confirm)
	{
		HideBackToMenuConfirmationPopup();// hide confirmation screen, show close UI pause, 

		if (confirm)
		{
			CloseUIPause();//close ui pause to unsub from all events 
			loadMenuEvent.RaiseEvent(mainMenu); //load main menu
		}
	}
	
	void HideBackToMenuConfirmationPopup()
	{
		selectionHandler.Unselect();
		pauseScreen.gameObject.SetActive(true); // Set pause screen to inactive

		// time is still set to 0 and Input is still set to menuInput 
		//going out from confirmaiton popup screen gets us back to the pause screen
	}
}