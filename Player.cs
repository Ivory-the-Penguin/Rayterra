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

    private Map _map;

    public Player(Map map)
    {
        Body = new(new Vector2(400, 100), new Vector2(10, 20));
        _map = map;
    }

    private bool TouchingMap()
    {
        List<AABB> check = _map.GetNearbyTileAABBs(new AABB(Body.Center - new Vector2(40), new Vector2(80, 78)));
        foreach (AABB box in check)
        {
            if (Body.Intersects(box))
            {
                return true;
            }
        }

        return false;
    }


    public void Update(float deltaTime, EntityManager manager)
    {
        _velocity.Y += GRAVITY * deltaTime;

        if (Input.IsKeyDown(KeyboardKey.Space))
        {
            _velocity.Y = -3f;
        }

        int loopAmount = (int)Math.Round(Math.Abs(_velocity.Y));
        for (int i = 0; i < loopAmount; i++)
        {
            Vector2 difference = new Vector2(0, _velocity.Y / loopAmount);

            Body.Position += difference;

            if (TouchingMap())
            {
                Body.Position -= difference;
                _velocity.Y = 0;
                return;
            }
        }

        int keyX = Convert.ToInt32(Input.IsKeyDown(KeyboardKey.D)) - Convert.ToInt32(Input.IsKeyDown(KeyboardKey.A));
        _velocity.X = keyX * SPEED * deltaTime;

        loopAmount = (int)Math.Round(Math.Abs(_velocity.X));
        for (int i = 0; i < loopAmount; i++)
        {
            Vector2 difference = new Vector2(_velocity.X / loopAmount, 0);

            Body.Position += difference;

            if (TouchingMap())
            {
                Body.Position -= difference;
                _velocity.X = 0;
                return;
            }
        }
    }

    public void Render()
    {
        Body.RenderHitbox();
    }
}