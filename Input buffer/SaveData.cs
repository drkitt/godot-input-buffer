using Godot;
using System;

/// <summary>
/// Contains the player's save data. Used with ResourceSaver and ResourceLoader to provide persistence.
/// </summary>
public class SaveData : Resource
{
    /// <summary> The highest score the player has achieved. </summary>
    public float HighScore { get; set; }
}