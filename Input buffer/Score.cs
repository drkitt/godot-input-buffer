using Godot;
using System;

/// <summary>
/// Keeps track of the player's score.
/// </summary>
public class Score : Label
{
    /// <summary> Passed to ToString to display the score. </summary>
    public static readonly string SCORE_FORMAT = "00000";

    /// <summary>
    /// Interface for getting the value the label is showing.
    /// </summary>
    public float DisplayedValue { get => _score; }

    /// <summary> How many points the player gains per second. </summary>
    [Export] private float _speed = 10;
    /// <summary> Whether changes to the score show up on screen. </summary>
    [Export] private bool _updating = false;
    /// <summary> The score display blinks whenever they player scores this many points. </summary>
    [Export] private float _blinkMilestone = 100;
    /// <summary> Makes the score blink when the player reaches a milestone. </summary>
    private AnimationPlayer _animator; [Export] private NodePath _animationPlayerPath;
    /// <summary> Keeps track of when the score should blink. </summary>
    private Timer _timer; [Export] private NodePath _timerPath;

    private float _score = 0;
    private bool _moving = false;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _animator = GetNode<AnimationPlayer>(_animationPlayerPath);
        _timer = GetNode<Timer>(_timerPath);
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous frame. </param>
    public override void _Process(float delta)
    {
        if (_moving)
        {
            _score += _speed * delta;
        }

        if (_updating)
        {
            // Display the score on the label.
            Text = _score.ToString(SCORE_FORMAT);
        }
    }

    /// <summary> Initializes the score. </summary>
    public void Start()
    {
        _moving = true;
        _updating = true;
        _score = 0;
        _timer.WaitTime = _blinkMilestone / _speed;

        _timer.Start();
    }

    /// <summary>
    /// Called when the game ends.
    /// </summary>
    private void _on_Dino_GotHit()
    {
        // If the score is currently blinking, stop doing that.
        _timer.Stop();
        if (_animator.PlaybackActive)
        {
            _animator.Stop();
            Text = _score.ToString(SCORE_FORMAT);
            Modulate = new Color(0xffffffff);
        }
    }

    /// <summary>
    /// Called when it's time for the score display to blink.
    /// </summary>
    private void _on_Timer_timeout()
    {
        _animator.Play("Blink");
        _timer.Start();
    }
}
