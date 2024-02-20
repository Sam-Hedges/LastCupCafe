using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerControllerSMV : MonoBehaviour {
    [SerializeField] private InputController _inputController;
    private Camera _mainCamera;
    private Rigidbody _rb;

    private LocomotionStateMachine _locomotionStateMachine;
    private InteractionStateMachine _interactionStateMachine;

    private void Awake() {
        _mainCamera = Camera.main;
        _rb = GetComponent<Rigidbody>();

        InitializeStateMachines();
    }        

    private void InitializeStateMachines() {
        _locomotionStateMachine = new LocomotionStateMachine();
        _interactionStateMachine = new InteractionStateMachine();

        // Initial states can be set here or based on game conditions
        _locomotionStateMachine.QueueState(new IdleState());
        // _interactionStateMachine.QueueState(new NoneState());
    }

    private void Update() {
    }


    // Other methods from your original script here...
}