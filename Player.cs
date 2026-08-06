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

    private Vector2 _velocity = new();

    public Player()
    {
        Body = new(new Vector2(100, 100), new Vector2(10, 20));
    }

    public void Update(float deltaTime, EntityManager manager)
    {
        Camera.DebugUpdate(deltaTime);

        _velocity.Y += GRAVITY * deltaTime;

        Body.Position += _velocity;
    }

    public void Render()
    {
        Body.RenderHitbox();
    }
}