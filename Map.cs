using System.Numerics;
using Raylib_cs;
using Rayterra.Core;

namespace Rayterra;

public class Map
{
    private const int TILE_SIZE = 16;
    private const int TILE_SCALE = 2;

    private const int WORLD_WIDTH = 1000;
    private const int WORLD_HEIGHT = 100;

    private TextureAtlas _atlas;

    private List<List<TileID>> _tiles = null!;

    public Map()
    {
        _atlas = Assets.InitAtlas("MapAtlas", "./Assets/RayterraAtlas.png", TILE_SIZE);

        GenMap(10);
    }

    public void GenMap(int scale)
    {
        _tiles = new(WORLD_WIDTH);

        Image perlin = Raylib.GenImagePerlinNoise(WORLD_WIDTH, WORLD_HEIGHT, 0, 0, scale);

        for (int i = 0; i < WORLD_WIDTH; i++)
        {
            List<TileID> cur = new(WORLD_HEIGHT);
            _tiles.Add(cur);
            for (int j = 0; j < WORLD_HEIGHT; j++)
            {
                Color color = Raylib.GetImageColor(perlin, i, j);

                if (color.R > 120)
                {
                    cur.Add(TileID.None);
                }
                else
                {
                    cur.Add(TileID.Dirt);
                }
            }
        }

    }

    public void Render()
    {
        for (int i = 0; i < WORLD_WIDTH; i++)
        {
            for (int j = 0; j < WORLD_HEIGHT; j++)
            {
                _atlas.RenderTile((int)_tiles[i][j], new Vector2(i * TILE_SIZE + 100, j * TILE_SIZE + 100), 1);
            }
        }
    }
}