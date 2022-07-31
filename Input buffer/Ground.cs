using Godot;
using System;

/// <summary>
/// A sprite representing the ground the dino runs across.
/// </summary>
public class Ground : Sprite
{
    /// <summary>
    /// Reference to the other ground sprite. When one sprite goes offscreen, it teleports behind the other one to give
    /// the appearance of a single endless ground sprite.
    /// </summary>
    private Ground _otherGround; [Export] private NodePath _otherGroundPath;

    /// <summary> The obstacles that can appear on the ground </summary>
    [Export] private PackedScene _cactus, _cactusClump, _pterodactyl;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _otherGround = GetNode<Ground>(_otherGroundPath);
    }

    /// <summary>
    /// Teleports the ground sprite behind the other one. Called when it exits the screen.
    /// </summary>
    /// Keep in mind that the colliders at the edge of the screen don't have classes, so this class uses collision
    /// layers to make sure that it only wraps around when it exits the screen
    private void WrapGround(Area2D area)
    {
        Position = Vector2.Right * _otherGround.Texture.GetWidth() + _otherGround.Position;
        SpawnObstacles();
    }

    private void SpawnObstacles(int startPos = 400)
    {
        // Min/max pixels between obstacles
        const int MIN_OFFSET = 80, MAX_OFFSET = 400;
        // The current position to spawn an obstacle at
        int currentPos = startPos;
        RandomNumberGenerator rng = new RandomNumberGenerator();

        while (currentPos <= Texture.GetWidth())
        {
            PackedScene obstacleScene;
            float r = rng.Randf();
            if (r < 1 / 3f)
            {
                obstacleScene = _cactus;
            }
            else if (r < 2 / 3f)
            {
                obstacleScene = _cactusClump;
            }
            else
            {
                obstacleScene = _pterodactyl;
            }


            // :O
            obstacleScene = _cactus;


            Node2D obstacle = obstacleScene.Instance<Node2D>();
            obstacle.Position = new Vector2(currentPos, obstacle.Position.y);
            CallDeferred("add_child", obstacle);

            currentPos += rng.RandiRange(MIN_OFFSET, MAX_OFFSET);
        }
    }
}