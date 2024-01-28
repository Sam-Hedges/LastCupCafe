using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class UICreditsRoller : MonoBehaviour
{
	[SerializeField, Tooltip("Set speed of a rolling effect")] private float speedPreset = 100f; //normal rolling speed
	[SerializeField, Tooltip("This is actuall speed of rolling")] private float speed = 100f; //actual speed of rolling
	[SerializeField] private bool rollAgain = false;

	[Header("References")]
	[SerializeField] private InputHandler inputReader = default;
	[SerializeField] private RectTransform textCredits = default;
	[SerializeField] private RectTransform mask = default;

	public event UnityAction OnRollingEnded;
	
	private float _expectedFinishingPoint;


	public void StartRolling()
	{
		speed = speedPreset;
		StartCoroutine(InitialOffset()); //This offset is needed to get true informations about rectangle and his mask
	}

	private void OnEnable()
	{
		inputReader.MoveEvent += OnMove;
	}

	private void OnDisable()
	{
		inputReader.MoveEvent -= OnMove;
	}

	void Update()
	{
		//This make rolling effect
		if (textCredits.anchoredPosition.y < _expectedFinishingPoint)
		{
			textCredits.anchoredPosition = new Vector2(textCredits.anchoredPosition.x, textCredits.anchoredPosition.y + speed * Time.deltaTime);
		}
		else if (_expectedFinishingPoint != 0) //this happend when rolling reach to end
		{
			RollingEnd();
		}
	}

	private IEnumerator InitialOffset()
	{
		yield return new WaitForSecondsRealtime(0.02f);

		inputReader.EnableGameplayInput();
		_expectedFinishingPoint = (textCredits.rect.height + mask.rect.height) / 2;

		textCredits.anchoredPosition = new Vector2(textCredits.anchoredPosition.x, -((textCredits.rect.height + mask.rect.height) / 2));
	}

	private void OnMove(Vector2 direction)
	{
		if (direction.y == 0f) //no horizontal movment
		{
			speed = speedPreset;
		}
		else if (direction.y > 0f) //upward movment
		{
			speed = speed * 2;
		}
		else //downward movment
		{
			speed = -speedPreset;
		}
	}

	private void RollingEnd()
	{
		if (rollAgain)
		{
			//reset postion of an element
			textCredits.anchoredPosition = new Vector2(textCredits.anchoredPosition.x, -((textCredits.rect.height + mask.rect.height) / 2));
		}
		else
		{
			OnRollingEnded.Invoke();
		}
	}
}
