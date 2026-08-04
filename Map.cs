using System.Numerics;
using Raylib_cs;
using Rayterra.Core;
using Rayterra.Helpers;

namespace Rayterra;

public class Map
{
    private const int TILE_SIZE = 8;

    private const int WORLD_WIDTH = 2100;
    private const int WORLD_HEIGHT = 600;

    private TextureAtlas _atlas;

    private List<List<TileID>> _tiles = null!;

    public Map()
    {
        _atlas = Assets.InitAtlas("MapAtlas", "./Assets/RayterraAtlas.png", TILE_SIZE);
    }

    public void ClearWorld()
    {
        _tiles = new(WORLD_WIDTH);
        for (int i = 0; i < WORLD_WIDTH; i++)
        {
            List<TileID> column = new(WORLD_HEIGHT);
            for (int j = 0; j < WORLD_HEIGHT; j++)
            {
                column.Add(TileID.None);
            }
            _tiles.Add(column);
        }
    }

    private const int GRASS_MAX_HEIGHT = 40;
    private const int GRASS_RANGE = 40;

    private const int STONE_MAX_HEIGHT = 100;
    private const int STONE_RANGE = 20;

    public void GenMap(float dirtScale, float stoneScale)
    {
        ClearWorld();

        // DIRT
        Image perlin = Raylib.GenImagePerlinNoise(WORLD_WIDTH, 1, 0, 0, dirtScale);

        for (int i = 0; i < WORLD_WIDTH; i++)
        {
            int height = (int)((float)Raylib.GetImageColor(perlin, i, 0).R / 255 * GRASS_RANGE) + GRASS_MAX_HEIGHT;

            for (int j = height; j < WORLD_HEIGHT; j++)
            {
                if (j == height)
                {
                    _tiles[i][j] = TileID.Grass;
                }
                else
                {
                    _tiles[i][j] = TileID.Dirt;
                }
            }
        }

        Raylib.UnloadImage(perlin);


        // STONE
        perlin = Raylib.GenImagePerlinNoise(WORLD_WIDTH, 1, 0, 0, stoneScale);

        for (int i = 0; i < WORLD_WIDTH; i++)
        {
            int height = (int)((float)Raylib.GetImageColor(perlin, i, 0).R / 255 * STONE_RANGE) + STONE_MAX_HEIGHT;

            for (int j = height; j < WORLD_HEIGHT; j++)
            {
                _tiles[i][j] = TileID.Stone;
            }
        }
        Raylib.UnloadImage(perlin);
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

        Raylib.DrawRectangleLines(0, 0, WORLD_WIDTH * TILE_SIZE, WORLD_HEIGHT * TILE_SIZE, Color.Red);
    }

    public Vector2 WorldToMapPosition(Vector2 position)
    {
        return position / TILE_SIZE;
    }
}