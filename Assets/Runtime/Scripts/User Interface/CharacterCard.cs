using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{
    [Header("References")]
    [Tooltip("")] [SerializeField]
    public Image _nameBackground;
    
    [Tooltip("")] [SerializeField]
    public TextMeshProUGUI _characterName;

    [Tooltip("")] [SerializeField]
    public RawImage _characterImage;
    
    [Tooltip("")] [SerializeField]
    public Image _inputTypeImage;
    
    [Tooltip("")] [SerializeField]
    public GameObject _isNotReadyIconGameObject;
    
    [Tooltip("")] [SerializeField]
    public GameObject _isReadyIconGameObject;
}
