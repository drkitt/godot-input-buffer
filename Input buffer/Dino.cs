using Godot;
using System;

/// <summary>
/// The game's brave protagonist.
/// </summary>
public class Dino : Sprite
{
    private AnimationPlayer _animator;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animator = GetNode<AnimationPlayer>("AnimationPlayer");
        
        _animator.Play("Run");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            GD.Print("jump");
        }
    }
}
