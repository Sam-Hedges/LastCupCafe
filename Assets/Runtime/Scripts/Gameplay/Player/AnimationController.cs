using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class AnimationController : MonoBehaviour
{
    [Header("Listening to Anchors")]
    private FloatAnchor _playerMotionBlendStateAnchor;
    
    [Header("Listening to Events")]
    private VoidEventChannelSO _playerPickupItemEventChannel;
    private VoidEventChannelSO _playerDropItemEventChannel;
    
    [Header("Visuals")]
    [SerializeField] private GameObject[] playerModels;
    [SerializeField] [Range(1, 10)] private float playerMotionBlendSpeed = 5;
    
    private GameObject _playerModel;
    private PlayerController _playerController;
    private RiggingHandler _riggingHandler;
    private Animator _animator;
    private static readonly int MotionBlend = Animator.StringToHash("MotionBlend");

    private void Awake() {
        _playerController = GetComponent<PlayerController>();
        
        // _playerModel = Instantiate(playerModels[Random.Range(0, playerModels.Length)], transform, true);
        _playerModel = Instantiate(playerModels[2], transform, true);
        Vector3 newPosition = Vector3.zero;
        newPosition.y = -1.15f;
        _playerModel.transform.localPosition = newPosition;
        
        _animator = _playerModel.GetComponent<Animator>();
        
        _riggingHandler = _playerModel.GetComponent<RiggingHandler>();
    }
    
    private void OnEnable() {
        // Motion Blend State
        _playerMotionBlendStateAnchor = ScriptableObject.CreateInstance<FloatAnchor>();
        _playerController.playerMotionBlendStateAnchor = _playerMotionBlendStateAnchor;
        _playerMotionBlendStateAnchor.OnAnchorProvided += OnPlayerMotionBlendState;
        
        // Pickup Item
        _playerPickupItemEventChannel = ScriptableObject.CreateInstance<VoidEventChannelSO>();
        _playerController.playerPickupItemEventChannel = _playerPickupItemEventChannel;
        _playerPickupItemEventChannel.OnEventRaised += OnPlayerPickupItem;
        
        // Drop Item
        _playerDropItemEventChannel = ScriptableObject.CreateInstance<VoidEventChannelSO>();
        _playerController.playerDropItemEventChannel = _playerDropItemEventChannel;
        _playerDropItemEventChannel.OnEventRaised += OnPlayerDropItem;
    }

    private void OnDisable() {
        // Motion Blend State
        _playerController.playerMotionBlendStateAnchor = null;
        _playerMotionBlendStateAnchor.OnAnchorProvided -= OnPlayerMotionBlendState;
        
        // Pickup Item
        _playerController.playerPickupItemEventChannel = null;
        _playerPickupItemEventChannel.OnEventRaised -= OnPlayerPickupItem;
        
        // Drop Item
        _playerController.playerDropItemEventChannel = null;
        _playerDropItemEventChannel.OnEventRaised -= OnPlayerDropItem;
    }
    
    private void OnPlayerMotionBlendState() {
        
        // TODO: Fix the motion blend state to use lerped values
        // TODO ISSUE: This function only gets called when the input value is changed so the lerp never gets finished
        
        float clampedValue = Mathf.Clamp(_playerMotionBlendStateAnchor.Value, 0, 1);
        float lerpedValue = Mathf.Lerp(_animator.GetFloat(MotionBlend), clampedValue, Time.deltaTime * playerMotionBlendSpeed);
        _animator.SetFloat(MotionBlend, clampedValue);
    }

    private void OnPlayerPickupItem() {
        _riggingHandler.PickupItem();
    }
    
    private void OnPlayerDropItem() {
        _riggingHandler.DropItem();
    }
}
