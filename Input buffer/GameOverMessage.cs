using Godot;
using System;

/// <summary>
/// Pops up when the game is over to let the player know they lost
/// </summary>
public class GameOverMessage : Control
{
    private TextureButton _retryButton; [Export] private NodePath _retryButtonPath;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _retryButton = GetNode<TextureButton>(_retryButtonPath);
    }

    /// <summary>
    /// Appear when the dino gets hit.
    /// </summary>
    private void _on_Dino_GotHit()
    {
        Visible = true;
        _retryButton.CallDeferred("grab_focus");
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
