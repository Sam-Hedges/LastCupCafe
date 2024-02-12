using UnityEngine;
using System;
using System.Collections;

public abstract class StateMachine<EState> : MonoBehaviour where EState : Enum {
    
    private BaseState<EState> _currentState;
    private BaseState<EState> _queuedState;
    private bool _isTransitioningState = false;

    protected virtual void Start() {
        if (_currentState != null) {
            _currentState.EnterState();
        }
    }

    protected virtual void Update() {
        if (_isTransitioningState || _currentState == null) return;

        if (_queuedState != null) {
            StartCoroutine(TransitionToState(_queuedState));
        } else {
            _currentState.UpdateState();
        }
    }

    protected virtual void FixedUpdate() {
        if (_currentState == null) return;

        _currentState.FixedUpdateState();
    }

    private IEnumerator TransitionToState(BaseState<EState> newState) {
        _isTransitioningState = true;
        if (_currentState != null) {
            _currentState.ExitState();
        }
        _currentState = newState;
        _currentState.EnterState();
        _queuedState = null; // Clear the queued state after transitioning
        _isTransitioningState = false;
        yield break;
    }

    public void QueueState(BaseState<EState> state) {
        if (_isTransitioningState || state == null) return;

        _queuedState = state;
    }
}