using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// The game's brave protagonist.
/// </summary>
public class Dino : KinematicBody2D
{
    private enum DinoState
    {
        Grounded,
        Jumping,
        Ducking,
        Dead
    }

    private readonly struct StateBehaviours
    {
        /// <summary> Method to call when entering this state. </summary>
        public readonly Action Enter;
        /// <summary> Method to call repeatedly while the state machine is in this state. </summary>
        public readonly Action<float> Process;
        /// <summary> Method to call when exiting this state. </summary>
        public readonly Action Exit;

        public StateBehaviours(Action enter = null, Action<float> process = null, Action exit = null)
        {
            // All of these callbacks are optional; if any callback is unspecified, we use a function that does nothing.
            Enter = (enter == null) ? () => { } : enter;
            Process = (process == null) ? (_) => { } : process;
            Exit = (exit == null) ? () => { } : exit;
        }
    }

    private static readonly string JUMP_ACTION = "ui_select";

    private DinoState _state = DinoState.Grounded;
    private AnimationPlayer _animator; [Export] private NodePath _animation_player_path;
    private Vector2 _velocity;
    private float _gravity;

    /// <summary> How many pixels per second squared the dino accelerates towards the ground at while rising. </summary>
    [Export] private float _regular_gravity = 2400f;
    /// <summary>
    /// How many pixels per second squared the dino accelerates towards the ground at if the player releases the jump 
    /// button while rising.
    /// </summary>
    [Export] private float _short_hop_gravity = 4800f;
    /// <summary>
    /// Pixels per second downward the dino moves the moment it jumps.
    /// Recall that the coordinate system has the positive y axis point down, so this should be negative.
    /// </summary>
    [Export] private float _initial_jump_speed = 800f;

    private Dictionary<DinoState, StateBehaviours> _stateMachine;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _stateMachine = new Dictionary<DinoState, StateBehaviours>
        {
            { DinoState.Grounded, new StateBehaviours
            (
                enter: () => GD.Print("Hello"),
                process: GroundedPhysicsProcess,
                exit: () => GD.Print("Goodbye")
            )},
            { DinoState.Jumping, new StateBehaviours
            (
                enter: () => GD.Print("hello"),
                process: JumpingPhysicsProcess,
                exit: () => GD.Print("goodbye")
            )},

        };

        _animator = GetNode<AnimationPlayer>(_animation_player_path);

        _stateMachine[_state].Enter();
    }

    /// <summary>
    /// Called during the physics processing step of the main loop.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous physics step. </param>
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        _stateMachine[_state].Process(delta);
    }

    /// <summary>
    /// Physics update for the grounded state.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous physics step. </param>
    private void GroundedPhysicsProcess(float delta)
    {
        if (Input.IsActionJustPressed(JUMP_ACTION))
        {
            _state = DinoState.Jumping;
            _velocity = _initial_jump_speed * Vector2.Up;
            _gravity = _regular_gravity;
            _animator.Play("Run");
        }
    }
    /// <summary>
    /// Physics update for the grounded state.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous physics step. </param>
    private void JumpingPhysicsProcess(float delta)
    {
        // Short-hop if the player releases the jump button while rising
        if (Input.IsActionJustReleased(JUMP_ACTION) && _velocity.Dot(Vector2.Up) > 0)
        {
            _gravity = _short_hop_gravity;
        }

        _velocity += _gravity * delta * Vector2.Down;

        // Reset the gravity once the dino begins falling after a short hop. 
        if (_velocity.Dot(Vector2.Up) < 0)
        {
            _gravity = _regular_gravity;
        }

        MoveAndSlide(_velocity, Vector2.Up);
        if (IsOnFloor())
        {
            _state = DinoState.Grounded;
        }
    }
}
