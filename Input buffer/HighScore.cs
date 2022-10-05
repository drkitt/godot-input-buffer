using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Records the best score the player has gotten.
/// </summary>
public class HighScore : Label
{
    private static string SAVE_PATH = "user://dino_game_save.json";
    private static string HIGH_SCORE_KEY = "high_score";

    /// <summary> Node keeping track of the score </summary>
    private Score _scoreNode; [Export] private NodePath _scoreNodePath = null;
    private float _highScore = 0;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _scoreNode = GetNode<Score>(_scoreNodePath);
        // if (ResourceLoader.Exists(SAVE_PATH))
        // {
        //     SaveData save = ResourceLoader.Load<SaveData>(SAVE_PATH, "Resource");
        //     _highScore = save.HighScore;
        // }
    }

    /// <summary>
    /// Called by the engine to respond to engine-level callbacks.
    /// </summary>
    /// <param name="what"> The notification that the engine has sent this node. </param>
    public override void _Notification(int what)
    {
        if (what == MainLoop.NotificationWmQuitRequest)
        {
            Dictionary<string, float> saveData = new Dictionary<string, float> { { HIGH_SCORE_KEY, _highScore } };
            File save = new File();
            save.Open(SAVE_PATH, File.ModeFlags.Write);
            save.StoreLine(JSON.Print(saveData));
            save.Close();
        }
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