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
        // Fly high, medium or low.
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        JavaClass.WeakRef();

        if (weFlyHigh)
        {
            // BALLIN'
            Position = new Vector2(Position.x, -160);
        }
        else
        {
            Position = new Vector2(Position.x, -48);
        }
    }
}
