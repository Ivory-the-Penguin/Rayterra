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
    private Map _map = null!;

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
        _map.Render();

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