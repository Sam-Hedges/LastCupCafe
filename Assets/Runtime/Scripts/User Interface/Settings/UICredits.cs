using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CreditsList
{
	public List<ContributerProfile> contributors = new List<ContributerProfile>();
}

[System.Serializable]
public class ContributerProfile
{
	public string name;
	public string contribution;
	public override string ToString()
	{
		return name + " - " + contribution;
	}
}

public class UICredits : MonoBehaviour
{
	public UnityAction OnCloseCredits;

	[SerializeField] private InputHandler inputReader = default;
	[SerializeField] private TextAsset creditsAsset;
	[SerializeField] private TextMeshProUGUI creditsText = default;
	[SerializeField] private UICreditsRoller creditsRoller = default;

	[Header("Listening on")]
	[SerializeField] private VoidEventChannelSO creditsRollEndEvent = default;

	private CreditsList _creditsList;

	private void OnEnable()
	{
		inputReader.MenuCloseEvent += CloseCreditsScreen;
		SetCreditsScreen();
	}

	private void OnDisable()
	{
		inputReader.MenuCloseEvent -= CloseCreditsScreen;
	}

	private void SetCreditsScreen()
	{
		creditsRoller.OnRollingEnded += EndRolling;
		FillCreditsRoller();
		creditsRoller.StartRolling();
	}

	private void CloseCreditsScreen()
	{
		creditsRoller.OnRollingEnded -= EndRolling;
		OnCloseCredits.Invoke();
	}

	private void FillCreditsRoller()
	{
		_creditsList = new CreditsList();
		string json = creditsAsset.text;
		_creditsList = JsonUtility.FromJson<CreditsList>(json);
		SetCreditsText();
	}

	private void SetCreditsText()
	{
		string creditsText = "";
		for (int i = 0; i < _creditsList.contributors.Count; i++)
		{
			if (i == 0)
				creditsText = creditsText + _creditsList.contributors[i].ToString();
			else
			{
				creditsText = creditsText + "\n" + _creditsList.contributors[i].ToString();

			}
		}
		this.creditsText.text = creditsText;
	}

	private void EndRolling()
	{
		if (creditsRollEndEvent != null)
			creditsRollEndEvent.RaiseEvent();
	}
}
