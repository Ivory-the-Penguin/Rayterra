using System.Numerics;
using Raylib_cs;

namespace Rayterra.Core;

public static class Input
{
    public static Vector2 MousePosition => Raylib.GetMousePosition();

    public static Vector2 MouseDelta => Raylib.GetMouseDelta();

    public static bool IsKeyDown(KeyboardKey key) => Raylib.IsKeyDown(key);

    public static bool IsKeyPressed(KeyboardKey key) => Raylib.IsKeyPressed(key);

    public static bool IsKeyReleased(KeyboardKey key) => Raylib.IsKeyReleased(key);

    public static bool IsMouseButtonDown(MouseButton button) => Raylib.IsMouseButtonDown(button);
    public static bool IsMouseButtonPressed(MouseButton button) => Raylib.IsMouseButtonPressed(button);
    public static bool IsMouseButtonReleased(MouseButton button) => Raylib.IsMouseButtonReleased(button);
}