using Godot;
using System;

/// <summary>
/// A cloud that lazily moves across the background
/// </summary>
public class Cloud : Node2D
{
    /// <summary> Just how lazily the cloud moves across the background </summary>
    [Export] private float _speed;
    /// <summary> When respawning, the cloud can go between 0 and this many units above the respawn point. </summary>
    [Export] private float _maxYOffset;
    /// <summary> Where to teleport to after leaving the screen </summary>
    private Position2D _respawnPoint; [Export] private NodePath _respawnPointPath;
    private bool _moving = false;
    private RandomNumberGenerator _rng;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _respawnPoint = GetNode<Position2D>(_respawnPointPath);
        _rng = new RandomNumberGenerator();
        _rng.Randomize();
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous frame. </param>
    public override void _Process(float delta)
    {
        if (_moving)
        {
            Position += Vector2.Left * _speed * delta;
        }
    }

    /// <summary>
    /// One of the methods of all time.
    /// </summary>
    public void Start() => _moving = true;

    /// <summary>
    /// Teleports the cloud to the right edge of the screen when it exits the screen.
    /// </summary>
    /// <param name="area"> The area that represents the edge of the screen. </param>
    /// Keep in mind that the colliders at the edge of the screen don't have classes, so this class uses collision
    /// layers to make sure that it only wraps around when it exits the screen.
    public void _on_Area2D_area_exited(Area2D area)
    {
        Position = _respawnPoint.Position;
        Position += Vector2.Down * _rng.RandiRange(0, (int)_maxYOffset);
    }
}