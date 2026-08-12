using System.Numerics;
using Raylib_cs;

namespace Rayterra.Core;

public class AABB
{
    public Vector2 Position;
    public Vector2 Size;

    public Vector2 Center => (Min + Max) * 0.5f;
    public Vector2 Min
    {
        get => Position;
        set => Position = value;
    }
    public Vector2 Max
    {
        get => Position + Size;
        set => Size = value - Position;
    }

    public Rectangle RayRect => new Rectangle(Position, Size);

    public static AABB Zero => new AABB(Vector2.Zero, Vector2.Zero);

    public AABB(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public AABB()
    {
        Position = Vector2.Zero;
        Size = Vector2.Zero;
    }

    public bool Intersects(AABB other)
    {
        if (!(Min.X < other.Max.X && Max.X > other.Min.X))
        {
            return false;
        }

        if (!(Min.Y < other.Max.Y && Max.Y > other.Min.Y))
        {
            return false;
        }

        return true;
    }

    public AABB GetCollisionRect(AABB other)
    {
        if (!Intersects(other))
        {
            return Zero;
        }

        Vector2 position = Vector2.Max(Min, other.Min);
        Vector2 size = Vector2.Min(Max, other.Max) - position;

        return new AABB(position, size);
    }

    public bool PointIntersects(Vector2 point)
    {
        if (!(point.X > Min.X && point.X < Max.X))
        {
            return false;
        }

        if (!(point.Y > Min.Y && point.Y < Max.Y))
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

    public bool IsZero()
    {
        return Position == Vector2.Zero && Size == Vector2.Zero;
    }
}