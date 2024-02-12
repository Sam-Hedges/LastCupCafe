using UnityEngine;
using System;

public abstract class BaseState<EState> where EState : Enum {
    
    // The key for the state
    public EState StateKey { get; private set; }
    protected BaseState(EState key) {
        StateKey = key;
    }

    /// <summary>
    /// Called at the start of the state
    /// </summary>
    public abstract void EnterState();
    
    /// <summary>
    /// Called at the end of the state
    /// </summary>
    public abstract void ExitState();
    
    /// <summary>
    /// Called every frame
    /// </summary>
    public virtual void UpdateState() { }
    
    /// <summary>
    /// Called every physics update
    /// </summary>
    public virtual void FixedUpdateState() { }
}

