using Godot;
using System;

/// <summary>
/// Toggles the input buffer on and off.
/// </summary>
public class BufferControl : Label
{
    /// <summary>
    /// <summary> Signal emitted when the buffer is toggled. </summary>
    /// </summary>
    /// <param name="on"> True when the buffer is turned on, false when it's turned off. </param>
    [Signal] private delegate void BufferToggled(bool on);

    private bool _bufferOn = true;
    private AnimationPlayer _animator; [Export] private NodePath _animatorPath;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        Modulate = new Color("00ffffff");
        _animator = GetNode<AnimationPlayer>(_animatorPath);
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <param name="delta"> The elapsed time since the previous frame. </param>
    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            _bufferOn = !_bufferOn;

            if (_bufferOn)
            {
                Text = "Input buffer ON";
            }
            else
            {
                Text = "Input buffer OFF";
            }
            if (_animator.IsPlaying())
            {
                _animator.Seek(0);
            }
            else
            {
                _animator.Play("Reveal");
            }

            EmitSignal(nameof(BufferToggled), new object[] { _bufferOn });
        }
    }
}