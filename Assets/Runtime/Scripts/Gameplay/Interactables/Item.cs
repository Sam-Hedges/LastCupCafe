using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private Material highlightMaterial;
    private MeshRenderer _meshRenderer;
    private Material[] _defaultMaterials;
    private Material[] _highlightMaterials;
    private PlayerController _playerController;

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _defaultMaterials = _meshRenderer.materials;
        InitHightlightMaterials();
    }

    private void FixedUpdate() {
        ValidateIfHighlighted();
    }
    
    private void ValidateIfHighlighted() {
        if (_playerController == null) return;
        
        // If the item has a parent that is a workstation, remove highlight if the player is not looking at the object
        if (transform.parent != null && transform.parent.GetComponent<Workstation>() != null) {
            // If the player is not looking at the parent workstation, remove highlight
            if (_playerController.recentlyCastInteractable != transform.parent.gameObject) {
                RemoveHighlight(); 
                return;
            }
        }
        
        // If the item is not a child of a workstation, remove highlight if the player is not looking at the object
        if (_playerController.recentlyCastInteractable != gameObject) RemoveHighlight();
    }
    
    private void InitHightlightMaterials() {
        _highlightMaterials = new Material[_defaultMaterials.Length + 1];
        Array.Copy(_defaultMaterials, _highlightMaterials, _defaultMaterials.Length);
        _highlightMaterials[_defaultMaterials.Length] = highlightMaterial;
    }

    public void AddHighlight(PlayerController playerController) {
        // Check if the object is already highlighted
        if (_meshRenderer.materials == _highlightMaterials) return;
        _meshRenderer.materials = _highlightMaterials;
        _playerController = playerController;
    }

    private void RemoveHighlight() {
        _meshRenderer.materials = _defaultMaterials;
        _playerController = null;
    }

    public void OnInteract() {
        throw new NotImplementedException();
    }
}
