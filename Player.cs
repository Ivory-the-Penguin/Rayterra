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
        _velocity.Y += GRAVITY * deltaTime;

        if (Input.IsKeyDown(KeyboardKey.Space))
        {
            _velocity.Y = -3f;
        }

        int keyX = Convert.ToInt32(Input.IsKeyDown(KeyboardKey.D)) - Convert.ToInt32(Input.IsKeyDown(KeyboardKey.A));
        _velocity.X = keyX * SPEED * deltaTime;

        for (int i = 0; i < 10; i++)
        {
            HandleCollisions(_velocity / 10);
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
            AABB cr = Body.GetCollisionRect(box);
            if (cr.IsZero()) continue;
            if (cr.Size.X > cr.Size.Y)
            {
                if (cr.Center.Y > Body.Center.Y)
                    Body.Position.Y -= cr.Size.Y;
                else
                    Body.Position.Y += cr.Size.Y;
                _velocity.Y = 0;
            }
            else
            {
                if (cr.Center.X > Body.Center.X)
                    Body.Position.X -= cr.Size.X;
                else
                    Body.Position.X += cr.Size.X;
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