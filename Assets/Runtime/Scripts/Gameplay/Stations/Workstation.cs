using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workstation : MonoBehaviour, IInteractable
{
    public GameObject currentlyStoredItem;
    [SerializeField] private Material highlightMaterial;
    private MeshRenderer _meshRenderer;
    private Material[] _defaultMaterials;
    private Coroutine _removeHighlightCoroutine;
    private float _interactCooldownSeconds = 0.25f;

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _defaultMaterials = _meshRenderer.materials;
    }
    
    public void OnHighlight() {
        // Check if the object is already highlighted
        foreach (Material material in _meshRenderer.materials) {
            if (material == highlightMaterial) return;
        }
        
        // Add highlight material a list of materials
        Material[] newMaterials = new Material[_defaultMaterials.Length + 1];
        Array.Copy(_defaultMaterials, newMaterials, _defaultMaterials.Length);
        newMaterials[_defaultMaterials.Length] = highlightMaterial;
        _meshRenderer.materials = newMaterials;
        
        // Start cooldown for removing highlight
        _interactCooldownSeconds = 0.25f;
        if (_removeHighlightCoroutine == null) _removeHighlightCoroutine = StartCoroutine(RemoveHighlight());
    }

    private IEnumerator RemoveHighlight() {
        yield return new WaitForSeconds(_interactCooldownSeconds);
        _meshRenderer.materials = _defaultMaterials;
        _removeHighlightCoroutine = null;
    }

    public void OnPlaceItem(GameObject newItem) {
        if (currentlyStoredItem != null) return; 
        
        currentlyStoredItem = newItem;
        currentlyStoredItem.transform.position = transform.position;
        currentlyStoredItem.transform.rotation = transform.rotation;
        currentlyStoredItem.transform.SetParent(transform);
        currentlyStoredItem.transform.localPosition = Vector3.up;
    }
    
    public GameObject OnRemoveItem() {
        if (currentlyStoredItem == null) return null;
        
        GameObject temp = currentlyStoredItem;
        currentlyStoredItem = null;
        return temp;
    }

    public void OnInteract() {
        if (currentlyStoredItem == null) return; // TODO: Implement not going to work without newItem sound
        
        
    }
}
