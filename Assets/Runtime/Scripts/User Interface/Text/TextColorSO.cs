using UnityEngine;

[CreateAssetMenu(fileName = "TextColorSettings", menuName = "User Interface/Text Color Settings", order = 1)]
public class TextColorSO : ScriptableObject
{
    public Color defaultColour = Color.white;
    public Color selectedColour = Color.yellow;
}