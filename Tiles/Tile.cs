using System.Numerics;
using Rayterra.Core;

namespace Rayterra.Tiles;

public abstract class Tile
{
    protected int _tileX;
    protected int _tileY;

    TextureAtlas _atlas;

    public Tile(int tileX, int tileY, TextureAtlas atlas)
    {
        _tileX = tileX;
        _tileY = tileY;
        _atlas = atlas;
    }

    public virtual void Render(Vector2 position)
    {
        _atlas.RenderTile(_tileX, _tileY, position, 3);
    }
}