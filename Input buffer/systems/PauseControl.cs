using Godot;
using System;

/// <summary>
/// 
/// </summary>
public class PauseControl : Node
{
    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {

    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous frame. </param>
    public override void _Process(float delta)
    {

    }

    public override void _Notification(int what)
    {
        base._Notification(what);

        switch (what)
        {
            case MainLoop.NotificationWmFocusOut: GetTree().Paused = true; break;
            case MainLoop.NotificationWmFocusIn: GetTree().Paused = false; break;
        }
    }
}