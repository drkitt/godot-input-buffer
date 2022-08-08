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
    /// <summary> The sprites used for the ground. </summary>
    private Node2D _ground1, _ground2; [Export] private NodePath _groundPath1, _groundPath2;
    private bool _moving = false;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _ground1 = GetNode<Node2D>(_groundPath1);
        _ground2 = GetNode<Node2D>(_groundPath2);
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous frame. </param>
    public override void _Process(float delta)
    {
        if (_moving)
        {
            _ground1.Position += Vector2.Left * _speed * delta;
            _ground2.Position += Vector2.Left * _speed * delta;

            _speed += _acceleration * delta;
        }
    }
}
