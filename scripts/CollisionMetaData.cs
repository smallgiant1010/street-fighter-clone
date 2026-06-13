using Godot;

public readonly struct CollisionMetaData
{
    public readonly Vector2 PositionOffset;
    public readonly Vector2 Size;
    public readonly float RotationDegrees;
    public CollisionMetaData(float xOffset, float yOffset, float degrees, float width, float height)
    {
        PositionOffset = new Vector2(xOffset, yOffset);
        RotationDegrees = degrees;
        Size = new Vector2(width, height);
    }
}