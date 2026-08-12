using System.Numerics;
using Raylib_cs;
using Rayterra.Core.Entity;
using Rayterra.Core;

namespace Rayterra;

public class Player : IEntity
{
    public Camera Camera { get; private set; } = new();

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


    public void Update(float deltaTime, EntityManager manager)
    {
        HandleHorizontalMovement(deltaTime);
        HandleVerticalMovement(deltaTime);

        for (int i = 0; i < 10; i++)
        {
            HandleCollisions(_velocity / 10);
        }

        Camera.Position = Body.Center - (Raylib.GetScreenCenter() / Camera.Zoom);

        if (Input.IsMouseButtonDown(MouseButton.Left))
        {
            _map.BreakTile(_map.WorldToMapPosition(Camera.Position + (Input.MousePosition / Camera.Zoom)));
        }
    }

    private void HandleVerticalMovement(float deltaTime)
    {
        int keyX = Convert.ToInt32(Input.IsKeyDown(KeyboardKey.D)) - Convert.ToInt32(Input.IsKeyDown(KeyboardKey.A));
        _velocity.X = keyX * Speed * deltaTime;
    }

    private void HandleHorizontalMovement(float deltaTime)
    {
        if (CoyoteTimer > 0) { CoyoteTimer -= deltaTime; }
        else { IsTouchingFloor = false; }

        if (IsTouchingFloor && Input.IsKeyPressed(KeyboardKey.Space))
        {
            _velocity.Y = -JumpForce;
        }

        _velocity.Y += Gravity * deltaTime;
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

    public void Render()
    {
        Body.RenderHitbox();
        _collisionRange.RenderHitbox(Color.Magenta, 1);
    }
}