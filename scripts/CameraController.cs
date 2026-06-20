using Godot;
using System;

public partial class CameraController : Camera2D
{
	[Export] private GameMap _gameMap;
	[Export] private Character1 _character1;
	private CollisionShape2D _characterCollisionShape;
	private Vector2 screenSize;
	public override void _Ready()
	{
		Position = new Vector2(0f, 0f);
		_gameMap.MapLoaded += OnMapLoaded;
		screenSize = GetViewport().GetVisibleRect().Size;
		_characterCollisionShape = _character1.GetNode<CollisionShape2D>("WorldCollisionShape2D");
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
		Vector2 characterPosition = _character1.GlobalPosition;
		Vector2 half = screenSize * 0.5f;
		float characterSize = ((RectangleShape2D)_characterCollisionShape.Shape).Size.X;

		float offset = 0;
		if ((characterPosition.X + characterSize) > (GlobalPosition.X + half.X))
		{
			offset = characterPosition.X + characterSize - (GlobalPosition.X + half.X);
		}
		else if ((characterPosition.X - characterSize) < (GlobalPosition.X - half.X))
		{
			offset = characterPosition.X - characterSize - (GlobalPosition.X - half.X);
		}

		GlobalPosition += new Vector2(offset, 0f);
	}
}
