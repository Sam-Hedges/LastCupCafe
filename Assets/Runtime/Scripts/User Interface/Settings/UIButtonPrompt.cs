using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonPrompt : MonoBehaviour
{

	[SerializeField] private Image interactionKeyBg = default;
	[SerializeField] private TextMeshProUGUI interactionKeyText = default;
	[SerializeField] private Sprite controllerSprite = default;
	[SerializeField] private Sprite keyboardSprite = default;
	[SerializeField] private string interactionKeyboardCode = default;
	[SerializeField] private string interactionJoystickKeyCode = default;

	public void SetButtonPrompt(bool isKeyboard)
	{
		if (!isKeyboard)
		{
			interactionKeyBg.sprite = controllerSprite;
			interactionKeyText.text = interactionJoystickKeyCode;
		}
		else
		{
			interactionKeyBg.sprite = keyboardSprite;
			interactionKeyText.text = interactionKeyboardCode;
		}
	}
}
