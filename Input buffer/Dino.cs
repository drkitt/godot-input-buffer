using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// The game's brave protagonist.
/// </summary>
public class Dino : KinematicBody2D
{
    /// <summary>
    /// After doing the jump to begin the game, we emit this signal so other nodes in the scene can finish the intro 
    /// animation.
    /// </summary>
    [Signal] private delegate void IntroJumpFinished();

    private enum DinoState
    {
        Idle,
        IntroAnimation,
        Grounded,
        Jumping,
        Ducking,
        Dead
    }

    private static readonly string JUMP_ACTION = "ui_select";
    private static readonly string DUCK_ACTION = "ui_down";

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
        _regularHitbox = GetNode<CollisionShape2D>(_regularHitboxPath);
        _duckingHitbox = GetNode<CollisionShape2D>(_duckingHitboxPath);

        _stateMachine = new StateMachine<DinoState>(
            new Dictionary<DinoState, StateSpec>
            {
                { DinoState.Idle, new StateSpec(enter: IdleEnter, update: IdlePhysicsProcess) },
                { DinoState.IntroAnimation, new StateSpec(
                    enter: IntroAnimationEnter, update: IntroAnimationPhysicsProcess, exit: IntroAnimationExit) },
                { DinoState.Grounded, new StateSpec(enter: GroundedEnter, update: GroundedPhysicsProcess)},
                { DinoState.Jumping, new StateSpec(enter: JumpingEnter, update: JumpingPhysicsProcess)},
                { DinoState.Ducking, new StateSpec(
                    enter: DuckingEnter, update: DuckingPhysicsProcess, exit: DuckingExit) },
            },
            DinoState.Idle
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

    // Idle state callback.
    private void IdleEnter()
    {
        _animator.Play("Idle + Jump");
    }
    private void IdlePhysicsProcess(float delta)
    {
        if (Input.IsActionJustPressed(JUMP_ACTION))
        {
            _stateMachine.TransitionTo(DinoState.IntroAnimation);
        }
    }

    // Intro animation state callbacks.
    private void IntroAnimationEnter()
    {
        _velocity = _initial_jump_speed * Vector2.Up;
        _gravity = _regular_gravity;
    }
    private void IntroAnimationPhysicsProcess(float delta)
    {
        // Move and detect collision with the ground.
        _velocity += _gravity * delta * Vector2.Down;
        MoveAndSlide(_velocity, Vector2.Up);
        if (IsOnFloor())
        {
            _stateMachine.TransitionTo(DinoState.Grounded);
        }
    }
    private void IntroAnimationExit()
    {
        EmitSignal(nameof(IntroJumpFinished), new object[0]);
    }

    // Grounded state callbacks.
    private void GroundedEnter()
    {
        _animator.Play("Run");
    }
    private void GroundedPhysicsProcess(float delta)
    {
        if (Input.IsActionJustPressed(JUMP_ACTION))
        {
            _stateMachine.TransitionTo(DinoState.Jumping);
        }
        else if (Input.IsActionPressed(DUCK_ACTION))
        {
            _stateMachine.TransitionTo(DinoState.Ducking);
        }
    }

    // Jumping state callbacks.
    private void JumpingEnter()
    {
        _velocity = _initial_jump_speed * Vector2.Up;
        _gravity = _regular_gravity;
        _animator.Play("Idle + Jump");
    }
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

    // Ducking state callbacks.
    private void DuckingEnter()
    {
        _animator.Play("Ducking");
        _regularHitbox.SetDeferred("disabled", true);
        _duckingHitbox.SetDeferred("disabled", false);
    }
    private void DuckingPhysicsProcess(float delta)
    {
        if (!Input.IsActionPressed(DUCK_ACTION))
        {
            _stateMachine.TransitionTo(DinoState.Grounded);
        }
    }
    private void DuckingExit()
    {
        _regularHitbox.SetDeferred("disabled", false);
        _duckingHitbox.SetDeferred("disabled", true);
    }
}
