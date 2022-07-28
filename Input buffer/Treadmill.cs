using Godot;
using System;

/// <summary>
/// Controls the ground and the obstacles that the dino runs through.
/// </summary>
public class Treadmill : Node2D
{
    /// <summary> Pixels per second the ground moves at. </summary>
    [Export] private float _speed = 10f;
    /// <summary> Pixels per second per second the ground accelerates at. </summary>
    [Export] private float _acceleration = 0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Position += Vector2.Left * _speed * delta;
        _speed += _acceleration * delta;
    }
}
