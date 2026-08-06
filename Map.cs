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

    private MapView _worldView;

    public Map()
    {
        _atlas = Assets.InitAtlas("MapAtlas", "./Assets/RayterraAtlas.png", TILE_SIZE);
        InitializeWorld();
    }

    public void InitializeWorld()
    {
        _worldView = new MapView(new MapPosition(0, 0), WORLD_WIDTH, WORLD_HEIGHT);

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
                column.Add(0);
            }
            _lightValues.Add(column);
        }

        ClearWorld();
    }

    public void ClearWorld()
    {
        foreach (MapPosition position in _worldView)
        {
            SetTile(position, TileID.Air);
            SetLightValue(position, 0);
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

        MapView caveView = new MapView(new MapPosition(0, CAVE_MAX_HEIGHT), WORLD_WIDTH, WORLD_HEIGHT - CAVE_MAX_HEIGHT);
        foreach (MapPosition position in caveView)
        {
            if (Raylib.GetImageColor(perlin, position.X, position.Y - CAVE_MAX_HEIGHT).R > (int)(caveExposure * 255))
            {
                SetTile(position, TileID.Air);
            }
        }

        Raylib.UnloadImage(perlin);

        // LIGHT
        SimulateLight(_worldView);
    }

    public void SimulateLight(MapView view)
    {
        Queue<(MapPosition Position, int Light)> queue = new();

        foreach (MapPosition position in view)
        {
            if (GetTile(position) == TileID.Air)
            {
                queue.Enqueue((position, LIGHT_VALUE_MAX));
            }
        }

        MapPosition neighbor;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (GetLightValue(current.Position) >= current.Light)
            {
                continue;
            }


            SetLightValue(current.Position, current.Light);

            if (current.Light == 1)
            {
                continue;
            }

            neighbor = new MapPosition(current.Position.X + 1, current.Position.Y);
            if (IsInMap(neighbor) && GetTile(neighbor) != TileID.Air)
            {
                queue.Enqueue((neighbor, current.Light - 1));
            }
            neighbor = new MapPosition(current.Position.X - 1, current.Position.Y);
            if (IsInMap(neighbor) && GetTile(neighbor) != TileID.Air)
            {
                queue.Enqueue((neighbor, current.Light - 1));
            }

            neighbor = new MapPosition(current.Position.X, current.Position.Y + 1);
            if (IsInMap(neighbor) && GetTile(neighbor) != TileID.Air)
            {
                queue.Enqueue((neighbor, current.Light - 1));
            }
            neighbor = new MapPosition(current.Position.X, current.Position.Y - 1);
            if (IsInMap(neighbor) && GetTile(neighbor) != TileID.Air)
            {
                queue.Enqueue((neighbor, current.Light - 1));
            }
        }
    }

    private const int LIGHT_VALUE_MAX = 9;

    public void Render(Camera camera)
    {
        foreach (MapPosition position in GetCameraView(camera))
        {
            Color lightColor = LightValueToColor(GetLightValue(position));

            TileID tile = GetTile(position);

            if (tile != TileID.Air)
            {
                _atlas.RenderTile((int)tile, MapPositionToWorldPosition(position), 1, lightColor);
            }
        }

        Raylib.DrawRectangleLines(0, 0, WORLD_WIDTH * TILE_SIZE, WORLD_HEIGHT * TILE_SIZE, Color.Red);
    }

    private Vector2 MapPositionToWorldPosition(MapPosition position)
    {
        return new Vector2(position.X * TILE_SIZE, position.Y * TILE_SIZE);
    }

    private Color LightValueToColor(int lightValue)
    {
        int value = Math.Clamp((int)((float)lightValue / LIGHT_VALUE_MAX * 255), 0, 255);
        return new Color(value, value, value);
    }

    public MapPosition WorldToMapPosition(Vector2 position)
    {
        return new MapPosition(Math.Clamp((int)(position.X / TILE_SIZE), 0, WORLD_WIDTH - 1), Math.Clamp((int)(position.Y / TILE_SIZE), 0, WORLD_HEIGHT - 1));
    }

    public TileID GetTile(MapPosition position)
    {
        if (!IsInMap(position))
        {
            return TileID.None;
        }

        return _tiles[position.X][position.Y];
    }

    public void SetTile(MapPosition position, TileID value)
    {
        if (!IsInMap(position))
        {
            return;
        }

        _tiles[position.X][position.Y] = value;
    }

    public int GetLightValue(MapPosition position)
    {
        if (!IsInMap(position))
        {
            return -1;
        }

        return _lightValues[position.X][position.Y];
    }

    public void SetLightValue(MapPosition position, int value)
    {
        if (!IsInMap(position))
        {
            return;
        }

        _lightValues[position.X][position.Y] = value;
    }

    public MapView GetCameraView(Camera camera)
    {
        MapPosition size = new MapPosition((int)(Raylib.GetScreenWidth() / camera.Object.Zoom / TILE_SIZE), (int)(Raylib.GetScreenHeight() / camera.Object.Zoom / TILE_SIZE));

        return new MapView(WorldToMapPosition(camera.Object.Target), size.X + 2, size.Y + 2);
    }

    public bool IsInMap(MapPosition position)
    {
        if (position.X < 0 || position.Y < 0)
        {
            return false;
        }

        if (position.X >= WORLD_WIDTH || position.Y >= WORLD_HEIGHT)
        {
            return false;
        }

        return true;
    }
}