using Godot;
using System;

/// <summary>
/// Controls the ground and the obstacles that the dino runs through.
/// </summary>
public class Treadmill : Node2D
{
    /// <summary> Pixels per second the ground moves at. </summary>
    [Export] private float _initialSpeed = 10f;
    /// <summary> Pixels per second per second the ground accelerates at. </summary>
    [Export] private float _acceleration = 0f;
    /// <summary> The sprites used for the ground. </summary>
    private Ground _ground1, _ground2; [Export] private NodePath _groundPath1 = null, _groundPath2 = null;
    private float _speed;
    private bool _moving = false;
    private Vector2 _initialGroundPosition;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _ground1 = GetNode<Ground>(_groundPath1);
        _ground2 = GetNode<Ground>(_groundPath2);

        _initialGroundPosition = _ground1.Position;
        _speed = _initialSpeed;
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

    /// <summary>
    /// Called when the player presses the retry button.
    /// </summary>
    private void _on_Retry_button_pressed()
    {
        _ground1.Position = _initialGroundPosition;
        _ground2.Position = _initialGroundPosition + Vector2.Right * _ground1.Texture.GetWidth();
        _speed = _initialSpeed;

        Start();
    }

    /// <summary>
    /// Starts the treadmill? This method's inner workings are a mystery.
    /// </summary>
    private void Start() => _moving = true;
}
