using Godot;
using System.Collections.Generic;

public partial class GameMap : Node2D
{
	private int Height { set; get; } = 1080;
	private int Width { set; get; } = 1920;
	private readonly Dictionary<int, string> map_paths = new()
	{
		{ 1, "res://assets/background_map_1.jpg"},
		{ 2, "res://assets/background_map_2.jpg"},
		{ 3, "res://assets/background_map_3.jpg"},
	};
	
	[ExportGroup("Dependencies")]
	[Export] private Sprite2D _backgroundMap;
	[Export] private StaticBody2D _ground;
	
	[Signal] public delegate void MapLoadedEventHandler(int width, int height);
	public override void _Ready()
	{
		_backgroundMap.Position = new Vector2(0f, 0f);
	}

	public void SetMap(int path_idx)
	{
		_backgroundMap.Texture = GD.Load<CompressedTexture2D>(map_paths[path_idx]);
		Width = _backgroundMap.Texture.GetWidth();
		Height = _backgroundMap.Texture.GetHeight();

		int groundHeight = Height / 2 - Height / 12;
		_ground.Position = new Vector2(0f, groundHeight);

		EmitSignal("MapLoaded", Width, Height);
	}
}   
