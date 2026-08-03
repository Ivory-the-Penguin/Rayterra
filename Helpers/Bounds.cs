using System.Numerics;
using Raylib_cs;

namespace Rayterra.Helpers;

public static class Bounds
{
    public static bool IsPointInBounds(Vector2 point, int width, int height)
    {
        if (0 > point.X || 0 > point.Y)
        {
            return false;
        }

        if (width < point.X || height < point.Y)
        {
            return false;
        }

        return true;
    }
}