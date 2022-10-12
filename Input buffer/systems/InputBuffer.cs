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

        GD.Print(InputMap.GetActionList("ui_accept"));
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous frame. </param>
    public override void _Process(float delta)
    {
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

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // This is a proof of concept for detecting whether an input is of a specific key or button.
        // By using InputMap.GetActionList, it should be possible to get the inputs corresponding to an action and check whether any of those keys/buttons were pressed!
        if (@event is InputEventKey)
        {
            InputEventKey eventKey = @event as InputEventKey;
            GD.Print(eventKey.Scancode);
            foreach (InputEvent input in InputMap.GetActionList("ui_accept"))
            {
                InputEventKey inputKey = input as InputEventKey;
                if (inputKey != null)
                {
                    GD.Print(eventKey.Scancode == inputKey.Scancode);
                }
            }
        }
        else if (@event is InputEventJoypadButton)
        {
            InputEventJoypadButton eventButton = @event as InputEventJoypadButton;
            GD.Print(eventButton.ButtonIndex);
            foreach (InputEvent input in InputMap.GetActionList("ui_accept"))
            {
                InputEventJoypadButton inputButton = input as InputEventJoypadButton;
                if (inputButton != null)
                {
                    GD.Print(eventButton.ButtonIndex == inputButton.ButtonIndex);
                }
            }
        }
        GD.Print();
    }
}