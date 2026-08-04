using Raylib_cs;
using System.Numerics;

using Rayterra.Core;

namespace Rayterra;

public class Camera
{
    private Camera2D _object;
    public Camera2D Object => _object;

    const int SPEED = 500;

    public Camera()
    {
        _object = new(Vector2.Zero, Vector2.Zero, 0, 2);
    }

    public void Update(float deltaTime)
    {
        int keyX = Convert.ToInt32(Input.IsKeyDown(KeyboardKey.D)) - Convert.ToInt32(Input.IsKeyDown(KeyboardKey.A));
        int keyY = Convert.ToInt32(Input.IsKeyDown(KeyboardKey.S)) - Convert.ToInt32(Input.IsKeyDown(KeyboardKey.W));

        _object.Target += new Vector2(keyX * SPEED * deltaTime, keyY * SPEED * deltaTime);
    }
}