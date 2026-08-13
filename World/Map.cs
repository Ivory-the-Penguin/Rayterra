using System.Numerics;
using Raylib_cs;
using Dara;

namespace Rayterra.World;

public record struct Tile(TileID ID, int LightValue)
{
    public Tile(TileID ID) : this(ID, 0) { }

    public bool IsSolid => ID >= 0;

    public static Tile None = new Tile(TileID.None);
}

public class Map
{
    public const int TileSize = 8;

    public const int WorldWidth = 2100;
    public const int WorldHeight = 600;

    private TextureAtlas _atlas;

    private List<List<Tile>> _tiles = null!;

    private MapView _worldView;

    public Map()
    {
        _atlas = Assets.InitAtlas("MapAtlas", "./Assets/RayterraAtlas.png", TileSize);
        InitializeWorld();
    }

    public void InitializeWorld()
    {
        _worldView = new MapView(new MapPosition(0, 0), WorldWidth, WorldHeight);

        _tiles = new List<List<Tile>>(WorldWidth);
        for (int i = 0; i < WorldWidth; i++)
        {
            List<Tile> column = new(WorldHeight);
            for (int j = 0; j < WorldHeight; j++)
            {
                column.Add(new Tile(TileID.None));
            }
            _tiles.Add(column);
        }
    }

    public void ClearWorld()
    {
        foreach (MapPosition position in _worldView) { this[position] = new Tile(TileID.Air); }
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
        Image perlin = Raylib.GenImagePerlinNoise(WorldWidth, 1, 0, 0, dirtScale);

        for (int x = 0; x < WorldWidth; x++)
        {
            int height = (int)(Raylib.ColorNormalize(Raylib.GetImageColor(perlin, x, 0)).X * GRASS_RANGE) + GRASS_MAX_HEIGHT;

            for (int y = height; y < WorldHeight; y++)
            {
                MapPosition position = new MapPosition(x, y);

                if (y == height)
                {
                    this[position] = new Tile(TileID.Grass);
                }
                else
                {
                    this[position] = new Tile(TileID.Dirt);
                }
            }
        }

        Raylib.UnloadImage(perlin);


        // STONE
        perlin = Raylib.GenImagePerlinNoise(WorldWidth, 1, 0, 0, stoneScale);

        for (int x = 0; x < WorldWidth; x++)
        {
            int height = (int)(Raylib.ColorNormalize(Raylib.GetImageColor(perlin, x, 0)).X * STONE_RANGE) + STONE_MAX_HEIGHT;

            for (int y = height; y < WorldHeight; y++)
            {
                this[new MapPosition(x, y)] = new Tile(TileID.Stone);
            }
        }
        Raylib.UnloadImage(perlin);

        // CAVES
        perlin = Raylib.GenImagePerlinNoise(WorldWidth, WorldHeight - CAVE_MAX_HEIGHT, 0, 0, caveScale);

        MapView caveView = new MapView(new MapPosition(0, CAVE_MAX_HEIGHT), WorldWidth, WorldHeight - CAVE_MAX_HEIGHT);
        foreach (MapPosition position in caveView)
        {
            if (Raylib.ColorNormalize(Raylib.GetImageColor(perlin, position.X, position.Y - CAVE_MAX_HEIGHT)).X > caveExposure)
            {
                this[position] = new Tile(TileID.Air);
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
            this[position] = new Tile(this[position].ID);

            if (this[position].ID == TileID.Air)
            {
                queue.Enqueue((position, LIGHT_VALUE_MAX));
            }
        }

        MapPosition neighbor;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (this[current.Position].LightValue >= current.Light)
            {
                continue;
            }

            this[current.Position] = new Tile(this[current.Position].ID, current.Light);

            if (current.Light == 1)
            {
                continue;
            }

            neighbor = current.Position + new MapPosition(1, 0);
            if (this[neighbor].IsSolid)
            {
                queue.Enqueue((neighbor, current.Light - 1));
            }
            neighbor = current.Position + new MapPosition(-1, 0);
            if (this[neighbor].IsSolid)
            {
                queue.Enqueue((neighbor, current.Light - 1));
            }

            neighbor = current.Position + new MapPosition(0, 1);
            if (this[neighbor].IsSolid)
            {
                queue.Enqueue((neighbor, current.Light - 1));
            }

            neighbor = current.Position + new MapPosition(0, -1);
            if (this[neighbor].IsSolid)
            {
                queue.Enqueue((neighbor, current.Light - 1));
            }
        }
    }

    private const int LIGHT_VALUE_MAX = 8;

    public void Render(Camera camera)
    {
        foreach (MapPosition position in GetCameraView(camera))
        {
            Tile tile = this[position];
            Color lightColor = LightValueToColor(tile.LightValue);

            if (tile.IsSolid)
            {
                _atlas.RenderTile((int)tile.ID, MapToWorldPosition(position), 1, lightColor);
            }
        }

        Raylib.DrawRectangleLines(0, 0, WorldWidth * TileSize, WorldHeight * TileSize, Color.Red);
    }

    public List<AABB> GetNearbyTileAABBs(AABB region)
    {
        MapView view = new MapView(WorldToMapPosition(region.Min), WorldToMapPosition(region.Max));

        List<AABB> hitboxList = new();
        foreach (MapPosition position in view)
        {
            if (this[position].IsSolid)
            {
                hitboxList.Add(new AABB(MapToWorldPosition(position), new Vector2(TileSize)));
            }
        }

        return hitboxList;
    }

    public Vector2 MapToWorldPosition(MapPosition position)
    {
        return new Vector2(position.X * TileSize, position.Y * TileSize);
    }

    private Color LightValueToColor(int lightValue)
    {
        float normalized = (float)lightValue / LIGHT_VALUE_MAX;
        return Raylib.ColorFromNormalized(new Vector4(normalized, normalized, normalized, 1));
    }

    public MapPosition WorldToMapPosition(Vector2 position)
    {
        return new MapPosition(Math.Clamp((int)(position.X / TileSize), 0, WorldWidth - 1), Math.Clamp((int)(position.Y / TileSize), 0, WorldHeight - 1));
    }

    public void SimulateLightAroundTile(MapPosition position)
    {
        SimulateLight(new MapView(
            position - new MapPosition(LIGHT_VALUE_MAX),
            position + new MapPosition(LIGHT_VALUE_MAX)));
    }

    public Tile this[MapPosition position]
    {
        get
        {
            if (!IsInMap(position))
            {
                return Tile.None;
            }

            return _tiles[position.X][position.Y];

        }

        set
        {
            if (!IsInMap(position))
            {
                return;
            }

            _tiles[position.X][position.Y] = value;

        }
    }

    public MapView GetCameraView(Camera camera)
    {
        MapPosition size = new MapPosition((int)(Raylib.GetScreenWidth() / camera.Zoom / TileSize), (int)(Raylib.GetScreenHeight() / camera.Zoom / TileSize));

        return new MapView(WorldToMapPosition(camera.Position), size.X + 2, size.Y + 2);
    }

    public bool IsInMap(MapPosition position)
    {
        if (position.X < 0 || position.Y < 0)
        {
            return false;
        }

        if (position.X >= WorldWidth || position.Y >= WorldHeight)
        {
            return false;
        }

        return true;
    }
}