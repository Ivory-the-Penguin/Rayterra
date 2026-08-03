using System.Numerics;
using Raylib_cs;
using Rayterra.Core;

namespace Rayterra;

public class Map
{
    private const int TILE_SIZE = 16;
    private const int TILE_SCALE = 3;
    private const int TILE_IN_RENDER = TILE_SIZE * TILE_SCALE;

    private TextureAtlas _atlas;

    private TileID[] _tiles =
    {
        TileID.Grass,
        TileID.Grass,
        TileID.Grass,
    };

    public Map()
    {
        _atlas = Assets.InitAtlas("MapAtlas", "./Assets/RayterraAtlas.png", TILE_SIZE);
    }

    public void Render()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            _atlas.RenderTile((int)_tiles[i], new Vector2(i * TILE_IN_RENDER + 100, 100), TILE_SCALE);
        }
    }
}