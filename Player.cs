using System.Numerics;
using Raylib_cs;
using Rayterra.Core.Entity;
using Rayterra.Helpers;

namespace Rayterra;

public class Player : IEntity
{
    public Camera Camera { get; private set; }

    public AABB Body { get; private set; }

    public Player()
    {
        Camera = new();
        Body = new(new Vector2(100, 100), new Vector2(10, 20));
    }

    public void Update(float deltaTime, EntityManager manager)
    {
        Camera.DebugUpdate(deltaTime);
    }

    public void Render()
    {
        Body.RenderHitbox();
    }
}