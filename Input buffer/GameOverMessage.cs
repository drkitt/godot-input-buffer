using Godot;
using System;

/// <summary>
/// Pops up when the game is over to let the player know they lost
/// </summary>
public class GameOverMessage : Control
{
    /// <summary>
    /// Appear when the dino gets hit.
    /// </summary>
    private void _on_Dino_GotHit()
    {
        Visible = true;
    }

    /// <summary>
    /// Restart the game when the button is pressed.
    /// </summary>
    private void _on_Retry_button_pressed()
    {
        GetTree().Paused = false;
        Visible = false;
    }
}
