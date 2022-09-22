using Godot;
using System;

/// <summary>
/// An experiment in giving a scene a 'global' animation player that can manipulate any node in the scene.
/// Will this reduce coupling (by replacing dependencies in code with AnimationPlayer tracks)? Or will it turn the scene
/// into an inscrutable mess (by nature of a god object that has access to everything)? Time to find out!
/// </summary>
/// A bit of elaboration: I think this is a good idea because the alternative is to skip this middle layer and make 
/// objects communicate with each other using their signals directly. So, there's gonna be coupling in the editor either
/// way, and I'd rather put it all in an AnimationPlayer than spread it across the scene. Plus, putting it in an 
/// animation makes it waaaay easier to manage timing.
public class DinoGameAnimator : AnimationPlayer
{
    /// <summary> Gets the game ready to play. </summary>
    private void _on_Dino_IntroJumpFinished()
    {
        Play("Intro animation");
    }
}