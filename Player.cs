using Raylib_cs;
using Rayterra.Core.Entity;

namespace Rayterra;

public class Player : IEntity
{
    public Camera Camera { get; private set; }

    public Player()
    {
        Camera = new();
    }

    public void Update(float deltaTime, EntityManager manager)
    {
        Camera.DebugUpdate(deltaTime);
    }

    public void Render()
    {

    }
}