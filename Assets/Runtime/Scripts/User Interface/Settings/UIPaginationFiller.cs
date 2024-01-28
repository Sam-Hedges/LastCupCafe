using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPaginationFiller : MonoBehaviour
{
	[SerializeField] private Image imagePaginationPrefab = default;
	[SerializeField] private Transform parentPagination = default;

	[SerializeField] private Sprite emptyPagination = default;
	[SerializeField] private Sprite filledPagination = default;

	HorizontalLayoutGroup _horizontalLayout = default;

	private List<Image> _instantiatedImages = default;
	[SerializeField]
	private int maxSpacing = 10;
	[SerializeField]
	private int minSpacing = 1;
	private void Awake()
	{
	}

	public void SetPagination(int paginationCount, int selectedPaginationIndex)
	{
		if (_instantiatedImages == null)
			_instantiatedImages = new List<Image>();

		//instanciate pagination images from the prefab
		int maxCount = Mathf.Max(paginationCount, _instantiatedImages.Count);
		if (maxCount > 0)
		{
			for (int i = 0; i < maxCount; i++)
			{
				if (i >= _instantiatedImages.Count)
				{
					Image instantiatedImage = Instantiate(imagePaginationPrefab, parentPagination);
					_instantiatedImages.Add(instantiatedImage);
				}

				if (i < paginationCount)
				{
					_instantiatedImages[i].gameObject.SetActive(true);

				}
				else
				{
					_instantiatedImages[i].gameObject.SetActive(false);

				}
			}
			SetCurrentPagination(selectedPaginationIndex);
		}

		_horizontalLayout = GetComponent<HorizontalLayoutGroup>();
		if (paginationCount < 10)
		{ _horizontalLayout.spacing = maxSpacing; }
		else if (paginationCount >= 10 && paginationCount < 20)
		{
			_horizontalLayout.spacing = (maxSpacing - minSpacing) / 2;
		}
		else
		{
			_horizontalLayout.spacing = minSpacing;
		}

	}

	public void SetCurrentPagination(int selectedPaginationIndex)
	{
		if (_instantiatedImages.Count > selectedPaginationIndex)
			for (int i = 0; i < _instantiatedImages.Count; i++)
			{
				if (i == selectedPaginationIndex)
				{
					_instantiatedImages[i].sprite = filledPagination;

				}
				else
				{
					_instantiatedImages[i].sprite = emptyPagination;
				}
			}
		else
			Debug.LogError("Error in pagination number");
	}
}
