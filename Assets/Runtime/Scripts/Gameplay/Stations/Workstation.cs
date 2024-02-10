using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workstation : MonoBehaviour, IInteractable
{
    public GameObject item;
    [SerializeField] private Material highlightMaterial;
    private MeshRenderer _meshRenderer;
    private Material[] _defaultMaterials;

    private void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _defaultMaterials = _meshRenderer.materials;
    }
    
    public void OnHighlight() {
        // Add highlight material a list of materials
        Material[] newMaterials = new Material[_defaultMaterials.Length + 1];
        Array.Copy(_defaultMaterials, newMaterials, _defaultMaterials.Length);
        newMaterials[_defaultMaterials.Length] = highlightMaterial;
        _meshRenderer.materials = newMaterials;
    }
    
    public void OnPlaceItem(GameObject newItem) {
        if (item != null) return;
        
        item = newItem;
        item.transform.position = transform.position;
        item.transform.rotation = transform.rotation;
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.up;
    }
    
    public GameObject OnRemoveItem() {
        if (item == null) return null;
        
        GameObject temp = item;
        item = null;
        return temp;
    }

    public void OnInteract() {
        if (item == null) return; // TODO: Implement not going to work without newItem sound
        
        
    }
}
