using Godot;

public partial class GameManager : Node
{
    [Export(PropertyHint.Range, "1,3")] private int Map_Index;
    [Export] private GameMap _gameMap;
    public override void _Ready()
    {
        _gameMap.SetMap(Map_Index);
    }
}
