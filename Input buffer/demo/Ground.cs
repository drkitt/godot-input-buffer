using Godot;
using System;
using System.Collections.Generic;
using ExtensionMethods;

/// <summary>
/// A sprite representing the ground the dino runs across.
/// </summary>
public class Ground : Sprite
{
    /// <summary> The obstacles that can appear on the ground </summary>
    [Export] private PackedScene _cactus = null, _cactusClump = null, _cactusBaby = null, _pterodactyl = null;
    /// <summary>
    ///  Where to spawn the first obstacle when the ground is initialized.
    /// Useful to give the player a bit of space at the very start of the game.
    /// </summary>
    [Export] private int _initialObstaclePos = 2000;
    /// <summary>
    /// Reference to the other ground sprite. When one sprite goes offscreen, it teleports behind the other one to give
    /// the appearance of a single endless ground sprite.
    /// </summary>
    private Ground _otherGround; [Export] private NodePath _otherGroundPath = null;
    /// <summary> The obstacles on this segment of the ground. </summary>
    private List<Node> _obstacles = new List<Node>();
    /// <summary> Whether it's legal to spawn pterodactyls. </summary>
    private bool _canSpawnPterodactyls = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _otherGround = GetNode<Ground>(_otherGroundPath);
        SpawnObstacles(_initialObstaclePos);
    }

    /// <summary>
    /// Removes all obstacles that belong to this slice of the ground.
    /// </summary>
    public void DespawnObstacles()
    {
        foreach (Node obstacle in _obstacles)
        {
            obstacle.QueueFree();
        }
        _obstacles.Clear();
    }

    /// <summary>
    /// Spawns obstacles along this slice of the ground. Note that this doesn't clear out existing obstacles.
    /// </summary>
    /// <param name="startPos"> Offset from the left edge of the ground at which to start placing obstacles. </param>
    private void SpawnObstacles(int startPos = 400)
    {
        // Min/max pixels between obstacles
        const int MIN_OFFSET = 450, MAX_OFFSET = 800;
        // The current position to spawn an obstacle at
        int currentPos = startPos;
        // Used to randomly spawn obstacles
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();

        // Spawn an obstacle, step along the ground, and repeat as long as we're still on the ground.
        while (currentPos <= Texture.GetWidth())
        {
            PackedScene obstacleScene;

            if (_canSpawnPterodactyls)
            {
                obstacleScene = rng.RandomValue<PackedScene>(new List<(ushort, PackedScene)>
                {
                    (2, _cactus),
                    (1, _cactusClump),
                    (1, _cactusBaby),
                    (2, _pterodactyl)
                });
            }
            else
            {
                obstacleScene = rng.RandomValue<PackedScene>(new List<(ushort, PackedScene)>
                {
                    (2, _cactus),
                    (1, _cactusClump),
                    (1, _cactusBaby),
                });
            }

            Node2D obstacle = obstacleScene.Instance<Node2D>();
            obstacle.Position = new Vector2(currentPos, obstacle.Position.y);
            CallDeferred("add_child", obstacle);
            _obstacles.Add(obstacle);

            int offset = rng.RandiRange(MIN_OFFSET, MAX_OFFSET);
            currentPos += offset;
        }
    }

    /// <summary>
    /// Teleports the ground sprite behind the other one. Called when it exits the screen.
    /// </summary>
    /// <param name="area"> The area that represents the edge of the screen. </param>
    /// Keep in mind that the colliders at the edge of the screen don't have classes, so this class uses collision
    /// layers to make sure that it only wraps around when it exits the screen.
    private void _on_Area2D_area_exited(Area2D area)
    {
        Position = Vector2.Right * _otherGround.Texture.GetWidth() + _otherGround.Position;
        DespawnObstacles();
        SpawnObstacles();
    }

    /// <summary>
    /// Resets the ground without messing with the position.
    /// </summary>
    private void _on_Retry_button_pressed()
    {
        DespawnObstacles();
        _canSpawnPterodactyls = false;
        SpawnObstacles(_initialObstaclePos);
    }

    /// <summary>
    /// Starts spawning pterodactyls.
    /// </summary>
    private void _on_Current_score_PterodactylTime()
    {
        _canSpawnPterodactyls = true;
    }
}
