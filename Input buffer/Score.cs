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
    private float _score = 0;
    private bool _moving = false;

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous frame. </param>
    public override void _Process(float delta)
    {
        if (_moving)
        {
            _score += _speed * delta;

            // Display the score on the label.
            Text = _score.ToString(SCORE_FORMAT);
        }
    }

    /// <summary> Initializes the score. </summary>
    public void Start()
    {
        _moving = true;
        _score = 0;
    }
}