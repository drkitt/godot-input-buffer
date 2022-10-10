using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Keeps track of recent inputs in order to make timing windows more flexible.
/// Intended use: Add this file to your project as an Autoload script and have other objects call the class' static 
/// methods.
/// (more on AutoLoad: https://docs.godotengine.org/en/stable/tutorials/scripting/singletons_autoload.html)
/// </summary>
public class InputBuffer : Node
{
    /// <summary>
    /// How many seconds ahead of time the player can make an input and have it still be recognized.
    /// I chose the value 0.15 because it imitates the 9-frame buffer window in the Super Smash Bros. Ultimate game.
    /// (source: https://smashboards.com/threads/ultimate-buffering-system.465269/)
    /// </summary>
    private static readonly float BUFFER_WINDOW = 0.15f;

    /// <summary> Tells when each input action was last pressed. </summary>
    private static Dictionary<string, ulong> _timestamps;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Process;

        // Initialize all dictionary entries.
        _timestamps = new Dictionary<string, ulong>();
        foreach (string actionName in InputMap.GetActions())
        {
            _timestamps.Add(actionName, 0);
        }
        /* With this node being autoloaded, the dictionary won't reflect any changes to the input map.
        If there's ever any need to change the input map at runtime (e.g. a control customization feature), make sure to
        also make this dictionary updatable. I'm keeping it non-updatable for now by the YAGNI principle, but if/when 
        you need it, try imitating Python's defaultdict to let every possible action start with timestamp 0.
        */
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous frame. </param>
    public override void _Process(float delta)
    {
/* The current design is to poll every action in the input map and update each timestamp every tick.
This is really unsatisfying, as I wanted to respond to each input event as it comes and only update the relevant timestamp.
If the engine has a way to find out what action an input event corresponds to, that'd be easy, but it looks like it only supports
finding out whether an input event corresponds to a specific action. 
I also tried keeping a map of timestamps and keyboard keys (which I could later generalize to other inputs), which I'd then 
use to make polling methods that manually check all the inputs associated with a given action... But it isn't possible to 
find the inputs associated with an action either!
So basically, for next time: Take another dig through the documentation and try to figure out what action is associated with
an input, but don't spend too long on it. After half an hour or so, suck it up and loop through the actions :)

btw I guess there could be multiple actions to an input. Keep that in mind when you're searching!

        foreach (string actionName in InputMap.GetActions())
        {
            if (Input.IsActionJustPressed(actionName))
            {
                // This value will wrap after roughly 500 million years. TODO: Implement a memory leak so the game crashes 
                // before that happens.
                _timestamps[actionName] = Time.GetTicksMsec();
            }
        }
    }
}