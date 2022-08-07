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

    private static readonly string JUMP_ACTION = "ui_select";

    private StateMachine<DinoState> _stateMachine;
    private AnimationPlayer _animator; [Export] private NodePath _animation_player_path;
    private CollisionShape2D _regularHitbox; [Export] private NodePath _regularHitboxPath;
    private CollisionShape2D _duckingHitbox; [Export] private NodePath _duckingHitboxPath;
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

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _animator = GetNode<AnimationPlayer>(_animation_player_path);

        _stateMachine = new StateMachine<DinoState>(
            new Dictionary<DinoState, StateSpec>
            {
                { DinoState.Grounded, new StateSpec(enter: GroundedEnter, process: GroundedPhysicsProcess)},
                { DinoState.Jumping, new StateSpec(enter: JumpingEnter, process: JumpingPhysicsProcess)},
            },
            DinoState.Grounded
        );

        _animator.Play("Idle + Jump");
    }

    /// <summary>
    /// Called during the physics processing step of the main loop.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous physics step. </param>
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        _stateMachine.Process(delta);
    }

    /// <summary>
    /// Called when entering the grounded state.
    /// </summary>
    private void GroundedEnter()
    {
        _animator.Play("Run");
    }
    /// <summary>
    /// Physics update for the grounded state.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous physics step. </param>
    private void GroundedPhysicsProcess(float delta)
    {
        if (Input.IsActionJustPressed(JUMP_ACTION))
        {
            _stateMachine.TransitionTo(DinoState.Jumping);
        }
    }
    /// <summary>
    /// Called when entering the jumping state.
    /// </summary>
    private void JumpingEnter()
    {
        _velocity = _initial_jump_speed * Vector2.Up;
        _gravity = _regular_gravity;
        _animator.Play("Idle + Jump");
    }
    /// <summary>
    /// Physics update for the jumping state.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous physics step. </param>
    private void JumpingPhysicsProcess(float delta)
    {
        // Short-hop if the player releases the jump button while rising
        if (Input.IsActionJustReleased(JUMP_ACTION) && _velocity.Dot(Vector2.Up) > 0)
        {
            _gravity = _short_hop_gravity;
        }

        // Reset the gravity once the dino begins falling after a short hop. 
        if (_velocity.Dot(Vector2.Up) < 0)
        {
            _gravity = _regular_gravity;
        }

        // Move and detect collision with the ground.
        _velocity += _gravity * delta * Vector2.Down;
        MoveAndSlide(_velocity, Vector2.Up);
        if (IsOnFloor())
        {
            _stateMachine.TransitionTo(DinoState.Grounded);
        }
    }
}
