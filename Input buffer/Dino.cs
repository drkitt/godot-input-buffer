using Godot;
using System;

/// <summary>
/// The game's brave protagonist.
/// </summary>
public class Dino : KinematicBody2D
{
    private static readonly string JUMP_ACTION = "ui_select";
    
    private enum DinoState
    {
        Idle,
        Jumping,
        Running,
        Dead
    }
    private DinoState _state = DinoState.Idle;
    private AnimationPlayer _animator; [Export] private NodePath _animation_player_path;
    private Vector2 _velocity;

    /// <summary> How many pixels per second squared the dino accelerates towards the ground at. </summary>
    [Export] private float _gravity = 2400f;
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
    }

    /// <summary>
    /// Called during the physics processing step of the main loop.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous physics step. </param>
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        switch (_state)
        {
            case DinoState.Idle:
            {
                if (Input.IsActionJustPressed(JUMP_ACTION))
                {
                    _state = DinoState.Jumping;
                    _velocity = _initial_jump_speed * Vector2.Up;
                    _animator.Play("Run");
                }
                break;
            }
            case DinoState.Jumping:
            {
                _velocity += _gravity * delta * Vector2.Down;
                MoveAndSlide(_velocity, Vector2.Up);
                if (IsOnFloor())
                {
                    _state = DinoState.Running;
                }
                break;
            }
            case DinoState.Running:
            {
                if (Input.IsActionJustPressed(JUMP_ACTION))
                {
                    _velocity = _initial_jump_speed * Vector2.Up;
                    _state = DinoState.Jumping;
                }
                break;
            }

            default: throw new InvalidOperationException("Unhandled state: " + _state);
        }
    }
}
