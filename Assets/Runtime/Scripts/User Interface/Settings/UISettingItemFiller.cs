using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISettingItemFiller : MonoBehaviour
{
	[SerializeField] private UIPaginationFiller pagination;
	[SerializeField] private TextMeshProUGUI currentSelectedOptionText;
	[SerializeField] private Image bg;
	[SerializeField] private Color colorSelected;
	[SerializeField] private Color colorUnselected;
	[SerializeField] private Sprite bgSelected;
	[SerializeField] private Sprite bgUnselected;
	[SerializeField] private MultiInputButton buttonNext;
	[SerializeField] private MultiInputButton buttonPrevious;

	public event UnityAction OnNextOption = delegate { };
	public event UnityAction OnPreviousOption = delegate { };

	public void FillSettingField_Localized(int paginationCount, int selectedPaginationIndex, string selectedOption)
	{
		pagination.SetPagination(paginationCount, selectedPaginationIndex);

		buttonNext.interactable = (selectedPaginationIndex < paginationCount - 1);
		buttonPrevious.interactable = (selectedPaginationIndex > 0);
	}
	
	public void FillSettingField(int paginationCount, int selectedPaginationIndex, string selectedOptionINT)
	{
		pagination.SetPagination(paginationCount, selectedPaginationIndex);
		currentSelectedOptionText.text = selectedOptionINT;

		buttonNext.interactable = (selectedPaginationIndex < paginationCount - 1);
		buttonPrevious.interactable = (selectedPaginationIndex > 0);
	}

	public void SelectItem()
	{
		bg.sprite = bgSelected;
		currentSelectedOptionText.color = colorSelected;
	}

	public void UnselectItem()
	{
		bg.sprite = bgUnselected;
		currentSelectedOptionText.color = colorUnselected;
	}

	public void NextOption()
	{
		OnNextOption.Invoke();
	}

	public void PreviousOption()
	{
		OnPreviousOption.Invoke();
	}

	public void SetNavigation(MultiInputButton buttonToSelectOnDown, MultiInputButton buttonToSelectOnUp)
	{
		MultiInputButton[] buttonNavigation = GetComponentsInChildren<MultiInputButton>();
		foreach (MultiInputButton button in buttonNavigation)
		{
			Navigation newNavigation = new Navigation();
			newNavigation.mode = Navigation.Mode.Explicit;
			if (buttonToSelectOnDown != null)
				newNavigation.selectOnDown = buttonToSelectOnDown;
			if (buttonToSelectOnDown != null)
				newNavigation.selectOnUp = buttonToSelectOnUp;
			newNavigation.selectOnLeft = button.navigation.selectOnLeft;
			newNavigation.selectOnRight = button.navigation.selectOnRight;
			button.navigation = newNavigation;
		}
	}
}
