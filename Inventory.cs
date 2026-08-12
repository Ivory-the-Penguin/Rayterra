using System.Numerics;
using Raylib_cs;

namespace Rayterra;

public class Inventory
{
    public void RenderUI()
    {
        DrawSlot(new Vector2(100, 100));
    }

    private float SlotSize = 20f;
    private float Padding = 5f;

    private void DrawSlot(Vector2 position)
    {
        Raylib.DrawRectangleRounded(new Rectangle(position, new Vector2(SlotSize)), 5f, 10, new Color(0, 0, 0, 144));
    }
}