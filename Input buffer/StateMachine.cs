using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Structures behaviour in terms of discrete states, each with enter, update, and exit methods.
/// </summary>
/// <typeparam name="StateEnum"> Enum representing the states that the machine can be in </typeparam>
public class StateMachine<StateEnum> where StateEnum : Enum
{
    /// <summary> Dictionary mapping each state to its methods. </summary>
    private readonly Dictionary<StateEnum, StateSpec> _behaviours;
    /// <summary> The current state. </summary>
    private StateEnum _state;

    /// <summary>
    /// Constructs the state machine containing the given behaviours.
    /// </summary>
    /// <param name="behaviours"> Dictionary mapping each state to its methods. </param>
    public StateMachine(Dictionary<StateEnum, StateSpec> behaviours, StateEnum startState)
    {
        _behaviours = behaviours;
        _state = startState;
        _behaviours[_state].Enter();
    }

    /// <summary>
    /// Calls the current state's Update method. Should be called in the container's _Update or _PhysicsUpdate 
    /// method.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous frame or physics step. </param>
    public void Update(float delta)
    {
        _behaviours[_state].Update(delta);
    }

    /// <summary>
    /// Transitions to another state.
    /// </summary>
    /// <param name="newState"> The state to transition to. </param>
    public void TransitionTo(StateEnum newState)
    {
        _behaviours[_state].Exit();
        _state = newState;
        _behaviours[_state].Enter();
    }
}

/// <summary>
/// Container for a state's methods (i.e. specification for the state's behaviour). 
/// Feel free to extend it instead of instantiating it if you want to have state-specific variables.
/// </summary>
public class StateSpec
{
    /// <summary> Method to call when entering this state. </summary>
    public readonly Action Enter;
    /// <summary> Method to call repeatedly while in this state. </summary>
    public readonly Action<float> Update;
    /// <summary> Method to call when exiting this state. </summary>
    public readonly Action Exit;

    /// <summary>
    /// Sets up the state's callbacks. If any callback is unspecified, it's replaced with a function that does nothing.
    /// </summary>
    /// <param name="enter"> Method to call when entering this state. </param>
    /// <param name="update"> Method to call repeatedly while in this state. </param>
    /// <param name="exit"> Method to call when exiting this state. </param>
    public StateSpec(Action enter = null, Action<float> update = null, Action exit = null)
    {
        // For each callback, use the given method if it was specified, otherwise use a function that does nothing.
        Action noOp = () => { };
        Action<float> floatNoOp = (_) => { };
        Enter = (enter == null) ? noOp : enter;
        Update = (update == null) ? floatNoOp : update;
        Exit = (exit == null) ? noOp : exit;
    }
}
