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
    private Camera _camera = null!;

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
        _camera = new();
    }

    private void Update()
    {
        Profiler.BeginProfile("update");

        float deltaTime = Raylib.GetFrameTime();

        _manager.Update(deltaTime);

        _camera.Update(deltaTime);

        Profiler.EndProfile();
    }

    private void Render()
    {
        Profiler.BeginProfile("render");

        Raylib.BeginDrawing();

        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode2D(_camera.Object);

        _manager.Render();
        _map.Render(_camera);

        Raylib.EndMode2D();

#if DEBUG
        RenderDebugUI();
#endif

        Profiler.EndProfile();

        Profiler.BeginProfile("present");
        Raylib.EndDrawing();
        Profiler.EndProfile();
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

        ImGui.NewLine();

        ImGui.Text($"Updating time: {Profiler.GetProfile("update").time} ms");
        ImGui.Text($"Rendering time: {Profiler.GetProfile("render").time} ms");
        ImGui.Text($"Present time: {Profiler.GetProfile("present").time} ms");

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