using UnityEngine;
using System;

/// <summary>
/// Base class for all states.
/// </summary>
/// <typeparam name="EState"></typeparam>
public abstract class BaseState<EState> where EState : Enum
{
    public BaseState(EState key)
    {
        StateKey = key;
    }
    
    public EState StateKey { get; private set; }
    public string Name => StateKey.ToString();
    
    // Abstract methods are required to override
    public abstract void Enter();
    public abstract void Exit();
    public abstract EState GetNextState();
    
    // Virtual methods are optional to override
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void OnTriggerEnter(Collider col) { }
    public virtual void OnTriggerStay(Collider col) { }
    public virtual void OnTriggerExit(Collider col) { }
    
    
}
