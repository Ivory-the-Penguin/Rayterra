using System.Numerics;
using Raylib_cs;
using Rayterra.Core;
using Rayterra.Helpers;

namespace Rayterra;

public class Map
{
    private const int TILE_SIZE = 16;

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

    public void Render(Camera camera)
    {
        Vector2 screenSize = Raylib.GetScreenCenter() * 2;

        Vector2 viewMin = WorldToMapPosition(camera.Object.Target);
        Vector2 viewSize = screenSize / camera.Object.Zoom / TILE_SIZE;

        for (int i = Math.Max(0, (int)viewMin.X); i < viewMin.X + viewSize.X; i++)
        {
            if (i >= _tiles.Count)
            {
                break;
            }

            for (int j = Math.Max(0, (int)viewMin.Y); j < viewMin.Y + viewSize.Y; j++)
            {
                if (j >= _tiles[i].Count)
                {
                    break;
                }

                _atlas.RenderTile((int)_tiles[i][j], new Vector2(i * TILE_SIZE, j * TILE_SIZE), 1);
            }
        }
    }

    public Vector2 WorldToMapPosition(Vector2 position)
    {
        return position / TILE_SIZE;
    }
}