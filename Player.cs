using System.Numerics;
using Raylib_cs;
using Rayterra.Core.Entity;
using Rayterra.Core;

namespace Rayterra;

public class Player : IEntity
{
    public Camera Camera { get; private set; } = new();

    public AABB Body { get; private set; }

    private const float GRAVITY = 10f;
    private const float SPEED = 100;

    public bool IsTouchingFloor { get; private set; } = false;

    private Vector2 _velocity = new();

    private AABB _collisionRange;

    private Map _map;

    public Player(Map map)
    {
        Body = new(new Vector2(400, 100), new Vector2(10, 20));

        _map = map;
        _collisionRange = new();
    }

    public float CoyoteTimer { get; private set; } = 0;
    public const float CoyoteDuration = 0.1f;

    public void Update(float deltaTime, EntityManager manager)
    {
        if (CoyoteTimer > 0) { CoyoteTimer -= deltaTime; }
        else { IsTouchingFloor = false; }

        _velocity.Y += GRAVITY * deltaTime;

        if (IsTouchingFloor && Input.IsKeyPressed(KeyboardKey.Space))
        {
            _velocity.Y = -3f;
        }

        int keyX = Convert.ToInt32(Input.IsKeyDown(KeyboardKey.D)) - Convert.ToInt32(Input.IsKeyDown(KeyboardKey.A));
        _velocity.X = keyX * SPEED * deltaTime;

        for (int i = 0; i < 10; i++)
        {
            HandleCollisions(_velocity / 10);
        }

        Camera.Position = Body.Center - (Raylib.GetScreenCenter() / Camera.Zoom);
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