using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorkstationType {
    Counter,
    CoffeePlant,
    CoffeeMachine,
    MilkFridge,
    SyrupCrate,
    ChocolateCreate,
    Grinder,
    KitchenSink,
    Kettle
}

public class Workstation : MonoBehaviour, IInteractable
{
    GameObject minigameCanvas;

    public WorkstationType workstationType;
    public GameObject currentlyStoredItem;
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
        // Remove highlight if the player is not looking at the object
        if (_playerController != null && _playerController.recentlyCastInteractable != gameObject) RemoveHighlight();
        
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

        if (currentlyStoredItem == null) return;
        currentlyStoredItem.GetComponent<Item>().AddHighlight(playerController);
    }

    private void RemoveHighlight() {
        _meshRenderer.materials = _defaultMaterials;
        _playerController = null;
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

    public virtual void OnInteract() {
        if (currentlyStoredItem == null) return; // TODO: Implement not going to work without newItem sound
        
        
        
    }

    public void GameUI(bool active)
    {
        minigameCanvas = gameObject.transform.GetChild(0).gameObject;
        if (active)
        {
            minigameCanvas.SetActive(true);
            Debug.Log("Activated Station");

        }
        else
        {
            minigameCanvas.SetActive(false);
            Debug.Log("Left Station");
        }

    }

    public virtual void MinigameButton(GameObject heldItem)
    {
        Debug.Log("Workstation active");
    }
    public virtual void MinigameTrigger(float delta, GameObject heldItem)
    {
        Debug.Log("Workstation active");
    }
    public virtual void MinigameStick(Vector2 input, GameObject heldItem)
    {
        Debug.Log("Workstation active");
    }
}
