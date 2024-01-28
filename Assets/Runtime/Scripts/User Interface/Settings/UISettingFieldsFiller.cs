using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettingFieldsFiller : MonoBehaviour
{
	[SerializeField] private UISettingItemFiller[] settingfieldsList = default;

	public void FillFields(List<SettingField> settingItems)
	{
		for (int i = 0; i < settingfieldsList.Length; i++)
		{
			if (i < settingItems.Count)
			{
				SetField(settingItems[i], settingfieldsList[i]);
				settingfieldsList[i].gameObject.SetActive(true);
			}
			else
			{
				settingfieldsList[i].gameObject.SetActive(false);
			}
		}

	}

	public void SetField(SettingField field, UISettingItemFiller uiField)
	{
		int paginationCount = 0;
		int selectedPaginationIndex = 0;
		string selectedOption = default;
		string fieldTitle = field.title;
		SettingFieldType fieldType = field.settingFieldType;

		switch (field.settingFieldType)
		{
			case SettingFieldType.AntiAliasing:

				break;
			case SettingFieldType.FullScreen:
				selectedPaginationIndex = IsFullscreen();
				paginationCount = 2;
				if (Screen.fullScreen)
					selectedOption = "On";
				else
					selectedOption = "Off";
				break;
			case SettingFieldType.ShadowDistance:

				break;
			case SettingFieldType.Resolution:

				break;
			case SettingFieldType.ShadowQuality:

				break;
			case SettingFieldType.VolumeMusic:
			case SettingFieldType.VolumeSFx:
				paginationCount = 10;
				selectedPaginationIndex = 5;
				selectedOption = "5";
				break;



		}
		uiField.FillSettingField(paginationCount, selectedPaginationIndex, selectedOption);


	}
	
	int IsFullscreen()
	{
		if (Screen.fullScreen)
		{
			return 0;
		}
		else
		{
			return 1;
		}

	}
}
