using Godot;
using System;

/// <summary>
/// Keeps track of the player's score.
/// </summary>
public class Score : Label
{
    /// <summary> Passed to ToString to display the score. </summary>
    public static readonly string SCORE_FORMAT = "00000";

    /// <summary> Signal to emit when the player has gotten enough points to start seeing pterodactyls. </summary>
    [Signal] private delegate void PterodactylTime();

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
    /// <summary> 
    /// When the player earns this many points, we emit a signal that tells the ground to start spawning pterodactyls. 
    /// </summary>
    [Export] private float _pterodactylMilestone = 500;
    /// <summary> Makes the score blink when the player reaches a milestone. </summary>
    private AnimationPlayer _animator; [Export] private NodePath _animationPlayerPath;
    /// <summary> Keeps track of when the score should blink. </summary>
    private Timer _blinkTimer; [Export] private NodePath _blinkTimerPath;
    /// <summary> Keeps track of when to emit the pterodactyl-spawning signal. </summary>
    private Timer _pterodactylTimer; [Export] private NodePath _pterodactylTimerPath;

    private float _score = 0;
    private bool _moving = false;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _animator = GetNode<AnimationPlayer>(_animationPlayerPath);
        _blinkTimer = GetNode<Timer>(_blinkTimerPath);
        _pterodactylTimer = GetNode<Timer>(_pterodactylTimerPath);
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
        _blinkTimer.WaitTime = _blinkMilestone / _speed;
        _pterodactylTimer.WaitTime = _pterodactylMilestone / _speed;

        _blinkTimer.Start();
        _pterodactylTimer.Start();
    }

    /// <summary>
    /// Called when the game ends.
    /// </summary>
    private void _on_Dino_GotHit()
    {
        // If the score is currently blinking, stop doing that.
        _blinkTimer.Stop();
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
    private void _on_Blink_timer_timeout()
    {
        _animator.Play("Blink");
        _blinkTimer.Start();
    }

    /// <summary>
    /// Called when it's time for the ground to start spawning pterodactyls.
    /// </summary>
    private void _on_Pterodactyl_timer_timeout()
    {
        EmitSignal(nameof(PterodactylTime), new object[0]);
    }
}
