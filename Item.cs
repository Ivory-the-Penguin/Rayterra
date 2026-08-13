using Rayterra.World;

namespace Rayterra;

public record struct Item(TileID ID, uint Amount)
{
    public bool IsEmpty => ID < 0 && Amount == 0;

    public static Item None = new Item(TileID.None, 0);
}

