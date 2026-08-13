using System.Numerics;
using Dara;
using ImGuiNET;
using Raylib_cs;
using Rayterra.World;

namespace Rayterra;

public record struct Item(TileID ID, uint Amount)
{
    public static bool operator true(Item item)
    {
        return item.ID >= 0 && item.Amount > 0;
    }

    public static bool operator false(Item item)
    {
        return item.ID < 0 || item.Amount == 0;
    }

    public static Item None = new Item(TileID.None, 0);
}

public class Inventory
{
    private const float TileScale = 3.5f;

    private TextureAtlas _atlas;

    private int _selected = 0;

    private Item[,] _inventory;

    public Item this[int x, int y]
    {
        get => _inventory[x, y];
        set => _inventory[x, y] = value;
    }

    public Item Selected
    {
        get => _inventory[_selected, 0];
        set => _inventory[_selected, 0] = value;
    }

    public Inventory()
    {
        _atlas = Assets.GetAtlas("MapAtlas");

        _inventory = new Item[9, 4];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                _inventory[i, j] = new Item(TileID.None, 0);
            }
        }
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
            Item item = _inventory[i, 0];

            DrawSlot(slotPosition, _selected == i);

            if (item.ID != TileID.None && item.Amount != 0)
            {
                Vector2 center = slotPosition + SlotSize / 2;

                _atlas.RenderTile((int)item.ID, center - new Vector2(TileScale * Map.TileSize / 2), TileScale, Color.White);
                Raylib.DrawTextEx(Raylib.GetFontDefault(), $"{item.Amount}", center + SlotSize / 6, 20, 2, Color.White);
            }

            slotPosition.X += Padding + SlotSize.X;
        }
    }

    private Vector2 SlotSize = new(65f);
    private float Padding = 20f;

    private void DrawSlot(Vector2 position, bool isSelected)
    {
        Raylib.DrawRectangleRounded(new Rectangle(position, SlotSize), 0.3f, 5, new Color(0, 0, 0, 144));
        if (isSelected)
        {
            Raylib.DrawRectangleRoundedLinesEx(new Rectangle(position, SlotSize), 0.3f, 5, 5f, new Color(0, 0, 0, 200));
        }
    }
}