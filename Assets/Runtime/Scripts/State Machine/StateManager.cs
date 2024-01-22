using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// References all states, handles transitions between states, and updates the current state.
/// </summary>
public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new();
    
    protected BaseState<EState> CurrentState;

    protected bool IsTransitioningState = false;

    private void Start()
    {
        if (CurrentState == null) { return; }
        CurrentState.Enter();
    }
    
    private void Update()
    {
        if (CurrentState == null) { return; }
        EState nextState = CurrentState.GetNextState();
        
        if (!IsTransitioningState && nextState.Equals(CurrentState.StateKey))
            CurrentState.Update();
        else if (!IsTransitioningState) StartCoroutine(TransitionToState(nextState));
    }
    
    private void FixedUpdate()
    {
        if (CurrentState == null) { return; }
        CurrentState.FixedUpdate();
    }

    private IEnumerator TransitionToState(EState stateKey)
    {
        IsTransitioningState = true;
        CurrentState.Exit();
        CurrentState = States[stateKey];
        CurrentState.Enter();
        IsTransitioningState = false;
        yield break;
    }
    
    private void OnTriggerEnter(Collider col)
    {
        if (CurrentState == null) { return; }
        CurrentState.OnTriggerEnter(col);
    }
    
    private void OnTriggerStay(Collider col)
    {
        if (CurrentState == null) { return; }
        CurrentState.OnTriggerStay(col);
    }
    
    private void OnTriggerExit(Collider col)
    {
        if (CurrentState == null) { return; }
        CurrentState.OnTriggerExit(col);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10f, 10f, 200f, 100f));
        string content = CurrentState != null ? CurrentState.Name : "(no current state)";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
        GUILayout.EndArea();
    }
}
