using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class SelectOnHover : MonoBehaviour
{
    // This method can be called from the EventTrigger
    public void SelectThisButton()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}