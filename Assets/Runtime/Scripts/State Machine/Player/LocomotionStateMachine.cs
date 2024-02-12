using UnityEngine;
using System;
using System.Collections;

public class LocomotionStateMachine : StateMachine<LocomotionState> {
    
    protected override void Start() {
        base.Start();
        QueueState(new IdleState()); // Example of setting an initial state
    } 
}

public class IdleState : BaseState<LocomotionState> {
    public IdleState() : base(LocomotionState.Idle) { }

    public override void EnterState() {
        Debug.Log("Entering Idle State");
    }

    public override void UpdateState() {
        Debug.Log("Updating Idle State");
    }

    public override void ExitState() {
        Debug.Log("Exiting Idle State");
    }
}

public class WalkState : BaseState<LocomotionState> {
    public WalkState() : base(LocomotionState.Walking) { }

    public override void EnterState() {
        Debug.Log("Entering Walking State");
    }

    public override void UpdateState() {
        Debug.Log("Updating Walking State");
    }

    public override void ExitState() {
        Debug.Log("Exiting Walking State");
    }
}

public class DashState : BaseState<LocomotionState> {
    public DashState() : base(LocomotionState.Dashing) { }

    public override void EnterState() {
        Debug.Log("Entering Dashing State");
    }

    public override void UpdateState() {
        Debug.Log("Updating Dashing State");
    }

    public override void ExitState() {
        Debug.Log("Exiting Dashing State");
    }
}