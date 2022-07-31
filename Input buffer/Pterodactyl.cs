using Godot;
using System;
using ExtensionMethods;

/// <summary>
/// A flying enemy.
/// </summary>
public class Pterodactyl : Sprite
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Fly low, medium, or high.
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        int weFlyHigh = rng.RandiRange(0, 2);

        switch (weFlyHigh)
        {
            case 0: // Ground level.
                Position = new Vector2(Position.x, -40);
                break;
            case 1: // Medium height, avoid by ducking.
                Position = new Vector2(Position.x, -80);
                break;
            case 2: // High (the dino can safely avoid this by doing nothing).
                Position = new Vector2(Position.x, -160);
                break;
        }
    }
}
