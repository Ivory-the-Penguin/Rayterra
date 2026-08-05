using System.Numerics;
using Raylib_cs;
using Rayterra.Core;

namespace Rayterra;

public class Map
{
    private const int TILE_SIZE = 8;

    private const int WORLD_WIDTH = 2100;
    private const int WORLD_HEIGHT = 600;

    private TextureAtlas _atlas;

    private List<List<TileID>> _tiles = null!;
    private List<List<int>> _lightValues = null!;

    public Map()
    {
        _atlas = Assets.InitAtlas("MapAtlas", "./Assets/RayterraAtlas.png", TILE_SIZE);
        InitializeWorld();
    }

    public void InitializeWorld()
    {
        _tiles = new List<List<TileID>>(WORLD_WIDTH);
        for (int i = 0; i < WORLD_WIDTH; i++)
        {
            List<TileID> column = new(WORLD_HEIGHT);
            for (int j = 0; j < WORLD_HEIGHT; j++)
            {
                column.Add(TileID.None);
            }
            _tiles.Add(column);
        }

        _lightValues = new List<List<int>>(WORLD_WIDTH);
        for (int i = 0; i < WORLD_WIDTH; i++)
        {
            List<int> column = new(WORLD_HEIGHT);
            for (int j = 0; j < WORLD_HEIGHT; j++)
            {
                column.Add(1);
            }
            _lightValues.Add(column);
        }
    }

    public void ClearWorld()
    {
        for (int i = 0; i < WORLD_WIDTH; i++)
        {
            for (int j = 0; j < WORLD_HEIGHT; j++)
            {
                _tiles[i][j] = TileID.None;
            }
        }
    }

    private const int GRASS_MAX_HEIGHT = 40;
    private const int GRASS_RANGE = 40;

    private const int STONE_MAX_HEIGHT = 100;
    private const int STONE_RANGE = 20;

    private const int CAVE_MAX_HEIGHT = 130;

    public void GenMap(float dirtScale, float stoneScale, float caveScale, float caveExposure)
    {
        ClearWorld();

        // DIRT
        Image perlin = Raylib.GenImagePerlinNoise(WORLD_WIDTH, 1, 0, 0, dirtScale);

        for (int x = 0; x < WORLD_WIDTH; x++)
        {
            int height = (int)((float)Raylib.GetImageColor(perlin, x, 0).R / 255 * GRASS_RANGE) + GRASS_MAX_HEIGHT;

            for (int y = height; y < WORLD_HEIGHT; y++)
            {
                if (y == height)
                {
                    _tiles[x][y] = TileID.Grass;
                }
                else
                {
                    _tiles[x][y] = TileID.Dirt;
                }
            }
        }

        Raylib.UnloadImage(perlin);


        // STONE
        perlin = Raylib.GenImagePerlinNoise(WORLD_WIDTH, 1, 0, 0, stoneScale);

        for (int x = 0; x < WORLD_WIDTH; x++)
        {
            int height = (int)((float)Raylib.GetImageColor(perlin, x, 0).R / 255 * STONE_RANGE) + STONE_MAX_HEIGHT;

            for (int y = height; y < WORLD_HEIGHT; y++)
            {
                _tiles[x][y] = TileID.Stone;
            }
        }
        Raylib.UnloadImage(perlin);

        // CAVES
        perlin = Raylib.GenImagePerlinNoise(WORLD_WIDTH, WORLD_HEIGHT - CAVE_MAX_HEIGHT, 0, 0, caveScale);

        for (int x = 0; x < WORLD_WIDTH; x++)
        {
            for (int y = CAVE_MAX_HEIGHT; y < WORLD_HEIGHT; y++)
            {
                if (Raylib.GetImageColor(perlin, x, y - CAVE_MAX_HEIGHT).R > (int)(caveExposure * 255))
                {
                    _tiles[x][y] = TileID.None;
                }
            }
        }
        Raylib.UnloadImage(perlin);
    }

    public void SimulateLight(Vector2 viewMin, Vector2 viewSize)
    {
        Queue<(int x, int y)> lighting = new();
    }

    private const int LIGHT_VALUE_MAX = 10;

    public void Render(Camera camera)
    {
        MapPosition size = new MapPosition((int)(Raylib.GetScreenWidth() / camera.Object.Zoom / TILE_SIZE), (int)(Raylib.GetScreenHeight() / camera.Object.Zoom / TILE_SIZE));

        MapView view = new MapView(WorldToMapPosition(camera.Object.Target), size.X + 1, size.Y + 2);

        foreach (MapPosition position in view)
        {
            int lightToRGB = Math.Clamp((int)((float)GetLightValue(position) / LIGHT_VALUE_MAX * 255), 0, 255);

            TileID tile = GetTile(position);

            if (tile != TileID.None)
            {
                _atlas.RenderTile((int)tile, new Vector2(position.X * TILE_SIZE, position.Y * TILE_SIZE), 1, new Color(lightToRGB, lightToRGB, lightToRGB));
            }
        }

        Raylib.DrawRectangleLines(0, 0, WORLD_WIDTH * TILE_SIZE, WORLD_HEIGHT * TILE_SIZE, Color.Red);
    }

    public MapPosition WorldToMapPosition(Vector2 position)
    {
        return new MapPosition(Math.Max(0, (int)(position.X / TILE_SIZE)), Math.Max(0, (int)(position.Y / TILE_SIZE)));
    }

    public TileID GetTile(MapPosition position)
    {
        if (position.X >= WORLD_WIDTH || position.Y >= WORLD_HEIGHT)
        {
            return TileID.None;
        }

        return _tiles[position.X][position.Y];
    }

    public int GetLightValue(MapPosition position)
    {
        if (position.X >= WORLD_WIDTH || position.Y >= WORLD_HEIGHT)
        {
            return -1;
        }

        return _lightValues[position.X][position.Y];
    }
}