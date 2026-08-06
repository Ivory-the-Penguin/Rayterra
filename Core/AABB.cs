using System.Numerics;
using Raylib_cs;

namespace Rayterra.Core;

public class AABB
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }

    public Vector2 Center => (Min + Max) * 0.5f;
    public Vector2 Min => Position;
    public Vector2 Max => Position + Size;

    public Rectangle RayRect => new Rectangle(Position, Size);

    public AABB(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public bool Intersects(AABB other)
    {
        if (!(Min.X <= other.Max.X && Max.X >= other.Min.X))
        {
            return false;
        }

        if (!(Min.Y <= other.Max.Y && Max.Y >= other.Min.Y))
        {
            return false;
        }

        return true;
    }

    public bool PointIntersects(Vector2 point)
    {
        if (!(point.X >= Min.X && point.X <= Max.X))
        {
            return false;
        }

        if (!(point.Y >= Min.Y && point.Y <= Max.Y))
        {
            return false;
        }

        return true;
    }


    public void RenderHitbox(Color color, float thickness)
    {
#if DEBUG
        Raylib.DrawRectangleLinesEx(RayRect, thickness, color);
#endif
    }

    public void RenderHitbox(Color color)
    {
        RenderHitbox(color, 2);
    }

    public void RenderHitbox()
    {
        RenderHitbox(Color.Red, 2);
    }
}