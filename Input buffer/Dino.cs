using Godot;
using System;

/// <summary>
/// The game's brave protagonist.
/// </summary>
public class Dino : Sprite
{
    /// <summary> Plays animations. </summary>
    private AnimationPlayer _animator;
    
    /// <summary> Pixels per second the dino currently moves downward. </summary>
    [Export] private float _speed = 0;
    /// <summary> How many pixels per second squared the dino accelerates towards the ground at. </summary>
    [Export] private float _gravity = 2400f;
    /// <summary> Pixels per second downward the dino moves the moment it jumps. </summary>
    [Export] private float _initial_jump_speed = -800f;

    /// <summary> 
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _animator = GetNode<AnimationPlayer>("AnimationPlayer");
        
        _animator.Play("Run");
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous frame. </param>
    public override void _Process(float delta)
    {
        // Apply forces. The dino is considered grounded if its transform's y is at least 0.
        Position += Vector2.Down * _speed * delta;
        if (Position.y >= 0)
        {
            Position = Vector2.Zero;
            _speed = 0;
        }
        else
        {
            _speed += _gravity * delta;
        }
        
        if (Input.IsActionJustPressed("ui_select"))
        {
            _speed = _initial_jump_speed;
        }
    }
}
