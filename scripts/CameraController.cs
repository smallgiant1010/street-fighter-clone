using Godot;
using System;

public partial class CameraController : Camera2D
{
	[Export] private GameMap _gameMap;
	public override void _Ready()
	{
		Position = new Vector2(0f, 0f);
		_gameMap.MapLoaded += OnMapLoaded;
	}

    private void OnMapLoaded(int width, int height)
    {
		int bottom = height / 2;
		int top = -1 * bottom;
		int right = (int)Mathf.Ceil(width / 2.0f);
		int left = -1 * right;

		LimitBottom = bottom;
		LimitTop = top;
		LimitRight = right;
		LimitLeft = left;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
