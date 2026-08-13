namespace Rayterra.World;

public record struct Tile(TileID ID, int LightValue)
{
    public Tile(TileID ID) : this(ID, 0) { }

    public bool IsSolid => ID >= 0;

    public static Tile None = new Tile(TileID.None);
}
