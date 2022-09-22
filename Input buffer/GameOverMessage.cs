using Godot;
using System;

/// <summary>
/// Pops up when the game is over to let the player know they lost
/// </summary>
public class GameOverMessage : TextureRect
{
    private void _on_Dino_GotHit()
    {
        Visible = true;
    }
}
