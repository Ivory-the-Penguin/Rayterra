using Rayterra.Core.Entity;

namespace Rayterra;

using Raylib_cs;
using ImGuiNET;
using rlImGui_cs;
using Rayterra.Core;
using System.Numerics;

public class Game
{
    private const int WINDOW_WIDTH = 1280;
    private const int WINDOW_HEIGHT = 720;
    private const string WINDOW_TITLE = "Rayterra";
    private const int FPS = 60;

    private EntityManager _manager = null!;
    private TextureAtlas _atlas = null!;

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
        Assets.LoadTexture("gameAtlas", "./Assets/RayterraAtlas.png");
    }

    private void InitSystems()
    {
        _manager = new();
        _atlas = new(Assets.GetTexture("gameAtlas"), 16);
    }

    private void Update()
    {
        float deltaTime = Raylib.GetFrameTime();

        _manager.Update(deltaTime);
    }

    private void Render()
    {
        Raylib.BeginDrawing();

        Raylib.ClearBackground(Color.Black);

        _manager.Render();

#if DEBUG
        RenderDebugUI();
#endif

        _atlas.RenderTile(1, 0, new Vector2(100, 100), 3);

        Raylib.EndDrawing();
    }

    private void RenderDebugUI()
    {
        rlImGui.Begin();

        ImGui.Text($"FPS: {Raylib.GetFPS()}");
        ImGui.Text($"Total Entities: {_manager.Count}");

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