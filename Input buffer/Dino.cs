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

    private enum GroundedState
    {
        Running,
        Ducking
    }

    private static readonly string JUMP_ACTION = "ui_select";
    private static readonly string DUCK_ACTION = "ui_down";

    private StateMachine<DinoState> _stateMachine;
    private StateMachine<GroundedState> _groundedStateMachine;
    private AnimationPlayer _animator; [Export] private NodePath _animation_player_path;
    private CollisionShape2D _regularHitbox; [Export] private NodePath _regularHitboxPath;
    private CollisionShape2D _duckingHitbox; [Export] private NodePath _duckingHitboxPath;
    private Vector2 _velocity;
    private float _gravity;
    private bool _ducking;

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
        _regularHitbox = GetNode<CollisionShape2D>(_regularHitboxPath);
        _duckingHitbox = GetNode<CollisionShape2D>(_duckingHitboxPath);

        _stateMachine = new StateMachine<DinoState>(
            new Dictionary<DinoState, StateSpec>
            {
                { DinoState.Grounded, new StateSpec(
                    enter: GroundedEnter,update: GroundedPhysicsProcess, exit: GroundedExit)},
                { DinoState.Jumping, new StateSpec(enter: JumpingEnter, update: JumpingPhysicsProcess)},
                { DinoState.Ducking, new StateSpec() },
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

        _stateMachine.Update(delta);
    }

    /// <summary>
    /// Called when entering the grounded state.
    /// </summary>
    private void GroundedEnter()
    {
        _ducking = false;
        _animator.Play("Run");
        _regularHitbox.SetDeferred("disabled", false);
        _duckingHitbox.SetDeferred("disabled", true);
    }
    /// <summary>
    /// Physics update for the grounded state.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous physics step. </param>
    private void GroundedPhysicsProcess(float delta)
    {
        if (Input.IsActionJustPressed(JUMP_ACTION))
        {
            // Jump.
            _stateMachine.TransitionTo(DinoState.Jumping);
        }
        else if (_ducking && !Input.IsActionPressed(DUCK_ACTION))
        {
            // Stand up and run.
            _ducking = false;
            _animator.Play("Run");
            _regularHitbox.SetDeferred("disabled", false);
            _duckingHitbox.SetDeferred("disabled", true);
        }
        else if (!_ducking && Input.IsActionPressed(DUCK_ACTION))
        {
            // Duck.
            _ducking = true;
            _animator.Play("Ducking");
            _regularHitbox.SetDeferred("disabled", true);
            _duckingHitbox.SetDeferred("disabled", false);
        }
    }
    /// <summary>
    /// Called when exiting the grounded state.
    /// </summary>
    private void GroundedExit()
    {
        // Restore the regular hitbox.
        _ducking = false;
        _regularHitbox.SetDeferred("disabled", false);
        _duckingHitbox.SetDeferred("disabled", true);
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
