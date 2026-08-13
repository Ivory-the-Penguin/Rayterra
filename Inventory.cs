using System.Numerics;
using Dara;
using ImGuiNET;
using Raylib_cs;
using Rayterra.World;

namespace Rayterra;

public record struct Item(TileID ID, int Amount);

public class Inventory
{
    private const float TileScale = 3.5f;

    private TextureAtlas _atlas;

    private int _selected = 0;

    public Inventory()
    {
        _atlas = Assets.GetAtlas("MapAtlas");
    }

    public void Update(float deltaTime)
    {
        for (KeyboardKey key = KeyboardKey.One; key <= KeyboardKey.Nine; key++)
        {
            if (Input.IsKeyPressed(key))
            {
                _selected = (int)key - 49;
            }
        }
    }

    public void RenderUI()
    {
        Vector2 slotPosition = new(Padding);
        for (int i = 0; i < 9; i++)
        {
            DrawSlot(slotPosition, _selected == i);

            _atlas.RenderTile(0, slotPosition + new Vector2(SlotSize / 2) - new Vector2(TileScale * Map.TileSize / 2), TileScale, Color.White);

            slotPosition.X += Padding + SlotSize;
        }
    }

    private float SlotSize = 65f;
    private float Padding = 20f;

    private void DrawSlot(Vector2 position, bool isSelected)
    {
        Raylib.DrawRectangleRounded(new Rectangle(position, new Vector2(SlotSize)), 0.3f, 5, new Color(0, 0, 0, 144));
        if (isSelected)
        {
            Raylib.DrawRectangleRoundedLinesEx(new Rectangle(position, new Vector2(SlotSize)), 0.3f, 5, 5f, new Color(0, 0, 0, 200));
        }
    }
}