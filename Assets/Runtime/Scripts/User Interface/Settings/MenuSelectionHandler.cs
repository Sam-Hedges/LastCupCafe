using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSelectionHandler : MonoBehaviour
{
	[SerializeField] private InputHandler inputReader;
	[SerializeField][ReadOnly] private GameObject defaultSelection;
	[SerializeField][ReadOnly] private GameObject currentSelection;
	[SerializeField][ReadOnly] private GameObject mouseSelection;

	private void OnEnable()
	{
		inputReader.MenuMouseMoveEvent += HandleMoveCursor;
		inputReader.MoveSelectionEvent += HandleMoveSelection;

		StartCoroutine(SelectDefault());
	}

	private void OnDisable()
	{
		inputReader.MenuMouseMoveEvent -= HandleMoveCursor;
		inputReader.MoveSelectionEvent -= HandleMoveSelection;
	}

	public void UpdateDefault(GameObject newDefault)
	{
		defaultSelection = newDefault;
	}

	/// <summary>
	/// Highlights the default element
	/// </summary>
	private IEnumerator SelectDefault()
	{
		yield return new WaitForSeconds(.03f); // Necessary wait otherwise the highlight won't show up

		if (defaultSelection != null)
			UpdateSelection(defaultSelection);
	}

	public void Unselect()
	{
		currentSelection = null;
		if (EventSystem.current != null)
			EventSystem.current.SetSelectedGameObject(null);
	}

	/// <summary>
	/// Fired by keyboard and gamepad inputs. Current selected UI element will be the ui Element that was selected
	/// when the event was fired. The _currentSelection is updated later on, after the EventSystem moves to the
	/// desired UI element, the UI element will call into UpdateSelection()
	/// </summary>
	private void HandleMoveSelection()
	{
		Cursor.visible = false;

		// Handle case where no UI element is selected because mouse left selectable bounds
		if (EventSystem.current.currentSelectedGameObject == null)
			EventSystem.current.SetSelectedGameObject(currentSelection);
	}

	private void HandleMoveCursor()
	{
		if (mouseSelection != null)
		{
			EventSystem.current.SetSelectedGameObject(mouseSelection);
		}

		Cursor.visible = true;
	}

	public void HandleMouseEnter(GameObject uiElement)
	{
		mouseSelection = uiElement;
		EventSystem.current.SetSelectedGameObject(uiElement);
	}

	public void HandleMouseExit(GameObject uiElement)
	{
		if (EventSystem.current.currentSelectedGameObject != uiElement)
		{
			return;
		}

		// keep selecting the last thing the mouse has selected 
		mouseSelection = null;
		EventSystem.current.SetSelectedGameObject(currentSelection);
	}

	/// <summary>
	/// Method interactable UI elements should call on Submit interaction to determine whether to continue or not.
	/// </summary>
	/// <returns></returns>
	public bool AllowsSubmit()
	{
		// if LMB is not down, there is no edge case to handle, allow the event to continue
		return !inputReader.LeftMouseDown()
			   // if we know mouse & keyboard are on different elements, do not allow interaction to continue
			   || mouseSelection != null && mouseSelection == currentSelection;
	}

	/// <summary>
	/// Fired by gamepad or keyboard navigation inputs
	/// </summary>
	/// <param name="uiElement"></param>
	public void UpdateSelection(GameObject uiElement)
	{
		if ((uiElement.GetComponent<MultiInputSelectableElement>() != null) || (uiElement.GetComponent<MultiInputButton>() != null))
		{
			mouseSelection = uiElement;
			currentSelection = uiElement;
		}
	}

	// Debug
	// private void OnGUI()
	// {
	//	 	GUILayout.Box($"_currentSelection: {(_currentSelection != null ? _currentSelection.name : "null")}");
	//	 	GUILayout.Box($"_mouseSelection: {(_mouseSelection != null ? _mouseSelection.name : "null")}");
	// }
	private void Update()
	{
		if ((EventSystem.current != null) && (EventSystem.current.currentSelectedGameObject == null) && (currentSelection != null))
		{

			EventSystem.current.SetSelectedGameObject(currentSelection);
		}
	}
}
