using System.Numerics;
using Raylib_cs;
using Dara.Entity;
using Dara;
using Rayterra.World;

namespace Rayterra;

public class Player : IEntity
{
    public Camera Camera { get; private set; } = new();
    public Inventory Inventory { get; private set; } = new();

    public AABB Body { get; private set; }

    private const float Gravity = 10f;
    private const float Speed = 150f;

    public bool IsTouchingFloor { get; private set; } = false;

    public float CoyoteTimer { get; private set; } = 0;
    public const float CoyoteDuration = 0.1f;

    public const float JumpForce = 4f;

    private Vector2 _velocity = new();

    private AABB _collisionRange;

    private Map _map;


    public Player(Map map)
    {
        Body = new(new Vector2(400, 100), new Vector2(10, 20));

        _map = map;
        _collisionRange = new();
    }

    public const int TileRange = 5;
    private MapPosition _selectedTilePosition;

    public void Update(float deltaTime, EntityManager manager)
    {
        HandleXMovement(deltaTime);
        HandleYMovement(deltaTime);

        HandleBlockStuff(deltaTime);

        Camera.Position = Body.Center - (Raylib.GetScreenCenter() / Camera.Zoom);
    }

    private void HandleBlockStuff(float deltaTime)
    {
        _selectedTilePosition = _map.WorldToMapPosition(Camera.MousePosition);

        if (Vector2.Distance(Body.Center, Camera.MousePosition) < Map.TileSize * TileRange)
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                Tile selectedTile = _map[_selectedTilePosition];

                if (selectedTile.IsSolid)
                {
                    if (Inventory.Selected.ID == selectedTile.ID || Inventory.Selected == Item.None)
                    {
                        Inventory.Selected = new Item(selectedTile.ID, Inventory.Selected.Amount + 1);
                        _map[_selectedTilePosition] = new Tile(TileID.Air);
                    }
                }
                else
                {
                    if (Inventory.Selected)
                    {
                        _map[_selectedTilePosition] = new Tile(Inventory.Selected.ID);
                        if (IsColliding())
                        {
                            _map[_selectedTilePosition] = new Tile(TileID.Air);
                        }
                        else
                        {
                            Inventory.Selected = new Item(Inventory.Selected.ID, Inventory.Selected.Amount - 1);
                            if (Inventory.Selected.Amount == 0)
                            {
                                Inventory.Selected = Item.None;
                            }
                        }
                    }
                }

                _map.SimulateLightAroundTile(_selectedTilePosition);
            }

        }

        Inventory.Update(deltaTime);
    }

    private void HandleXMovement(float deltaTime)
    {
        int keyX = Convert.ToInt32(Input.IsKeyDown(KeyboardKey.D)) - Convert.ToInt32(Input.IsKeyDown(KeyboardKey.A));
        _velocity.X = keyX * Speed * deltaTime;

        for (int i = 0; i < 10; i++)
        {
            HandleCollisions(new Vector2(_velocity.X / 10, 0));
        }
    }

    private void HandleYMovement(float deltaTime)
    {
        if (CoyoteTimer > 0) { CoyoteTimer -= deltaTime; }
        else { IsTouchingFloor = false; }

        if (IsTouchingFloor && Input.IsKeyDown(KeyboardKey.Space))
        {
            _velocity.Y = -JumpForce;
        }

        _velocity.Y += Gravity * deltaTime;

        for (int i = 0; i < 10; i++)
        {
            HandleCollisions(new Vector2(0, _velocity.Y / 10));
        }
    }

    private void HandleCollisions(Vector2 velocity)
    {
        _collisionRange.Position = Body.Center - new Vector2(25);
        _collisionRange.Size = new Vector2(50);

        Body.Position += velocity;
        List<AABB> nearbyTileBoxes = _map.GetNearbyTileAABBs(_collisionRange);
        foreach (AABB box in nearbyTileBoxes)
        {
            AABB collisionRect = Body.GetCollisionRect(box);
            if (collisionRect.IsZero()) continue;
            if (collisionRect.Size.X > collisionRect.Size.Y)
            {
                if (collisionRect.Center.Y > Body.Center.Y)
                {
                    Body.Position.Y -= collisionRect.Size.Y;
                    IsTouchingFloor = true;
                    CoyoteTimer = CoyoteDuration;
                }
                else
                {
                    Body.Position.Y += collisionRect.Size.Y;
                }
                _velocity.Y = 0;
            }
            else
            {
                if (collisionRect.Center.X > Body.Center.X)
                {
                    Body.Position.X -= collisionRect.Size.X;
                }
                else
                {
                    Body.Position.X += collisionRect.Size.X;
                }
                _velocity.X = 0;

            }
        }
    }

    private bool IsColliding()
    {
        _collisionRange.Position = Body.Center - new Vector2(25);
        _collisionRange.Size = new Vector2(50);

        List<AABB> nearbyTileBoxes = _map.GetNearbyTileAABBs(_collisionRange);
        foreach (AABB box in nearbyTileBoxes)
        {
            AABB collisionRect = Body.GetCollisionRect(box);
            if (collisionRect.IsZero()) continue;

            return true;
        }

        return false;
    }

    public void Render()
    {
        Body.RenderHitbox();
        _collisionRange.RenderHitbox(Color.Magenta, 1);

        if (_map[_selectedTilePosition].IsSolid)
        {
            Raylib.DrawRectangleRoundedLinesEx(new Rectangle(_map.MapToWorldPosition(_selectedTilePosition), new Vector2(Map.TileSize)), 0.2f, 5, 1.5f, Color.Yellow);
        }
    }
}