using Godot;
using System;

/// <summary>
/// A cactus obstacle for the dino to jump over
/// </summary>
public class Cactus : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Randomly choose one of the sprites
        RandomNumberGenerator rng = new RandomNumberGenerator();
        Godot.Collections.Array sprites = GetTree().GetNodesInGroup("Sprites");
        int spriteIndex = rng.RandiRange(0, sprites.Count - 1);
        for (int i = 0; i < sprites.Count; i++)
        {
            ((Sprite)sprites[i]).Visible = (i == spriteIndex);
        }
    }
}