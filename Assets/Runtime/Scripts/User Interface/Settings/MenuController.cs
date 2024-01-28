using UnityEngine;

public class MenuController : MonoBehaviour
{
	[SerializeField] private InputHandler inputReader;
	[SerializeField] private GameObject menuPrefab;
	private GameObject _menuInstance;

	private void OpenMenu()
	{
		if (_menuInstance == null)
			_menuInstance = Instantiate(menuPrefab);

		_menuInstance.SetActive(true);
		inputReader.EnableMenuInput();
	}

	private void UnpauseMenu()
	{
		_menuInstance.SetActive(false);
		inputReader.EnableGameplayInput();
	}
}
