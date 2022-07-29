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
    /// <summary> 
    /// The sprites used for the ground. This class swaps their positions to give the appearance of a single continuous
    /// ground sprite. _activeGround refers to the sprite that the dino is currently running on, and _inactiveGround 
    /// refers to the other one, so _groundPath1 should refer to the sprite that the dino initially runs on.
    /// </summary>
    private Node2D _activeGround, _inactiveGround; [Export] private NodePath _groundPath1, _groundPath2; 
    
    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _activeGround = GetNode<Node2D>(_groundPath1);
        _inactiveGround = GetNode<Node2D>(_groundPath2);
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous frame. </param>
    public override void _Process(float delta)
    {
        _activeGround.Position += Vector2.Left * _speed * delta;
        _inactiveGround.Position += Vector2.Left * _speed * delta;
        
        _speed += _acceleration * delta;
    }
}
