using Rayterra.Core.Entity;

namespace Rayterra;

using Raylib_cs;
using ImGuiNET;
using rlImGui_cs;
using Rayterra.Core;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

public class Game
{
    private const int WINDOW_WIDTH = 1280;
    private const int WINDOW_HEIGHT = 720;
    private const string WINDOW_TITLE = "Rayterra";
    private const int FPS = 60;

    private EntityManager _manager = null!;
    private Map _map = null!;

    private Camera2D _camera;

    public Game()
    {
        InitWindow();
        LoadTextures();

        InitSystems();
    }

    private void InitWindow()
    {
        Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, WINDOW_TITLE);
        Raylib.SetTargetFPS(FPS);
        Raylib.ToggleFullscreen();

        rlImGui.Setup(true);
    }

    private void LoadTextures()
    {
    }

    private void InitSystems()
    {
        _manager = new();
        _map = new();

        _camera = new(Vector2.Zero, Raylib.GetScreenCenter(), 0, 2);
    }

    private void Update()
    {
        float deltaTime = Raylib.GetFrameTime();

        int keyX = Convert.ToInt32(Input.IsKeyDown(KeyboardKey.D)) - Convert.ToInt32(Input.IsKeyDown(KeyboardKey.A));
        int keyY = Convert.ToInt32(Input.IsKeyDown(KeyboardKey.S)) - Convert.ToInt32(Input.IsKeyDown(KeyboardKey.W));

        const int SPEED = 10;

        _camera.Target += new Vector2(keyX * SPEED, keyY * SPEED);

        _manager.Update(deltaTime);
    }

    private void Render()
    {
        Raylib.BeginDrawing();

        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode2D(_camera);

        _manager.Render();
        _map.Render();

        Raylib.EndMode2D();

#if DEBUG
        RenderDebugUI();
#endif

        Raylib.EndDrawing();
    }

    private int _scale = 10;

    private void RenderDebugUI()
    {
        rlImGui.Begin();

        ImGui.Text($"FPS: {Raylib.GetFPS()}");
        ImGui.Text($"Total Entities: {_manager.Count}");

        ImGui.NewLine();

        ImGui.InputInt("Scale", ref _scale);
        if (ImGui.IsItemDeactivatedAfterEdit())
        {
            _map.GenMap(_scale);
        }

        rlImGui.End();
    }

    private void Cleanup()
    {
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }

    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            Update();
            Render();
        }
        Cleanup();
    }
}