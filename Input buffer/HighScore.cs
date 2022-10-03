using Godot;
using System;

/// <summary>
/// Records the best score the player has gotten.
/// </summary>
public class HighScore : Label
{
    /// <summary> Node keeping track of the score </summary>
    private Score _scoreNode; [Export] private NodePath _scoreNodePath = null;
    private float _highScore = 0;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _scoreNode = GetNode<Score>(_scoreNodePath);
    }

    /// <summary>
    /// Called when the game ends.
    /// </summary>
    private void _on_Dino_GotHit()
    {
        Update(_scoreNode.DisplayedValue);
    }

    /// <summary>
    /// Updates the high score.
    /// </summary>
    /// <param name="newScore"> 
    /// The score the player just got. Will be displayed if it's higher than the current high score. 
    /// </param>
    private void Update(float newScore)
    {
        if (newScore > _highScore)
        {
            _highScore = newScore;
            Text = "HI " + _highScore.ToString("00000");
        }
    }
}