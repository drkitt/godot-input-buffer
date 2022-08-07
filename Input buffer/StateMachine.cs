using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Structures behaviour in terms of discrete states, each with enter, process, and exit methods.
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
    /// Calls the current state's Process method. Should be called in the container's _Process or _PhysicsProcess 
    /// method.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous frame or physics step. </param>
    public void Process(float delta)
    {
        _behaviours[_state].Process(delta);
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
/// </summary>
public class StateSpec
{
    /// <summary> Method to call when entering this state. </summary>
    public readonly Action Enter;
    /// <summary> Method to call when exiting this state. </summary>
    public readonly Action Exit;
    /// <summary> Method to call repeatedly while in this state. </summary>
    private readonly Action<float> _process;

    /// <summary>
    /// Sets up the state's callbacks. If any callback is unspecified, it's replaced with a function that does nothing.
    /// </summary>
    /// <param name="enter"> Method to call when entering this state. </param>
    /// <param name="process"> Method to call repeatedly while in this state. </param>
    /// <param name="exit"> Method to call when exiting this state. </param>
    public StateSpec(Action enter = null, Action<float> process = null, Action exit = null)
    {
        // For each callback, use the given method if it was specified, otherwise use a function that does nothing.
        Action noOp = () => { };
        Action<float> floatNoOp = (_) => { };
        Enter = (enter == null) ? noOp : enter;
        _process = (process == null) ? floatNoOp : process;
        Exit = (exit == null) ? noOp : exit;
    }

    /// <summary>
    /// Method to call repeatedly while in this state. 
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous update. </param>
    /// We use this method instead of directly exposing the callback in order to let a derived class wrap it with other 
    /// logic.
    public virtual void Process(float delta)
    {
        _process(delta);
    }
}

/// <summary>
/// Container for a state's methods and a nested state machine (i.e. substate machine). Used to create a hierarchical 
/// state machine.
/// </summary>
/// <typeparam name="SubstateEnum"> Enum representing the sub-states that this state can be in. </typeparam>
public class StateSpec<SubstateEnum> : StateSpec where SubstateEnum : Enum
{
    /// <summary> State machine that's updated while this state is active. </summary>
    private readonly StateMachine<SubstateEnum> _substateMachine;

    /// <summary>
    /// Sets up the state's callbacks and substate machine.
    /// </summary>
    /// <param name="substateMachine"> State machine that's updated while this state is active. </param>
    /// <param name="enter"> Method to call when entering this state. </param>
    /// <param name="process"> Method to call repeatedly while in this state. </param>
    /// <param name="exit"> Method to call when exiting this state. </param>
    /// <returns></returns>
    public StateSpec
    (
        StateMachine<SubstateEnum> substateMachine,
        Action enter = null,
        Action<float> process = null,
        Action exit = null
    ) : base(enter, process, exit)
    {
        _substateMachine = substateMachine;
    }

    /// <summary>
    /// Method to call repeatedly while in this state. 
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous update. </param>
    /// Remember how Process in the base class was a method instead of a public delegate? It was so this class could 
    /// call _substateMachine.Process in its Process method :)
    public override void Process(float delta)
    {
        base.Process(delta);
        _substateMachine.Process(delta);
    }
}