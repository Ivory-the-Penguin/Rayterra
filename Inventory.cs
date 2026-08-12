using System.Numerics;
using Raylib_cs;

namespace Rayterra;

public class Inventory
{
    public void RenderUI()
    {
        Vector2 slotPosition = new(Padding);
        for (int i = 0; i < 9; i++)
        {
            DrawSlot(slotPosition);
            slotPosition.X += Padding + SlotSize;
        }
    }

    private float SlotSize = 65f;
    private float Padding = 20f;

    private void DrawSlot(Vector2 position)
    {
        Raylib.DrawRectangleRounded(new Rectangle(position, new Vector2(SlotSize)), 0.3f, 5, new Color(0, 0, 0, 144));
    }
}