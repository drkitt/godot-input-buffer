using Godot;
using System;

/// <summary>
/// Pauses the game when the window loses focus.
/// </summary>
public class PauseControl : Node
{
    /// <summary>
    /// Called by the engine to respond to engine-level callbacks.
    /// </summary>
    /// <param name="what"> The notification that the engine has sent this node. </param>
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