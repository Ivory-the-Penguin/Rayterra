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

    private Player _player = null!;

    public Game()
    {
        InitWindow();
        LoadTextures();

        InitSystems();
        InitEntities();
    }
    public void InitEntities()
    {
        _player = new();

        _manager.AddEntity(_player);
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

        _map.GenMap(_dirtScale, _stoneScale, _caveScale, _caveExposure);
    }

    private void Update()
    {
        Profiler.BeginProfile("update");

        float deltaTime = Raylib.GetFrameTime();

        _manager.Update(deltaTime);

        Profiler.EndProfile();
    }

    private void Render()
    {
        Profiler.BeginProfile("render");

        Raylib.BeginDrawing();

        Raylib.ClearBackground(Color.SkyBlue);

        Raylib.BeginMode2D(_player.Camera.RaylibCamera);

        _manager.Render();
        _map.Render(_player.Camera);

        Raylib.EndMode2D();

#if DEBUG
        RenderDebugUI();
#endif

        Profiler.EndProfile();

        Profiler.BeginProfile("present");
        Raylib.EndDrawing();
        Profiler.EndProfile();
    }

    private float _dirtScale = 0.008f;
    private float _stoneScale = 0.03f;
    private float _caveScale = 13.2f;
    private float _caveExposure = 0.55f;

    private void RenderDebugUI()
    {
        rlImGui.Begin();

        ImGui.Text($"FPS: {Raylib.GetFPS()}");
        ImGui.Text($"Total Entities: {_manager.Count}");

        ImGui.NewLine();

        ImGui.SliderFloat("Dirt Scale", ref _dirtScale, 0.001f, 0.2f);
        if (ImGui.IsItemEdited())
        {
            _map.GenMap(_dirtScale, _stoneScale, _caveScale, _caveExposure);
        }

        ImGui.SliderFloat("Stone Scale", ref _stoneScale, 0.001f, 0.2f);
        if (ImGui.IsItemEdited())
        {
            _map.GenMap(_dirtScale, _stoneScale, _caveScale, _caveExposure);
        }

        ImGui.NewLine();

        ImGui.SliderFloat("Cave Scale", ref _caveScale, 1.0f, 20.0f);
        if (ImGui.IsItemEdited())
        {
            _map.GenMap(_dirtScale, _stoneScale, _caveScale, _caveExposure);
        }

        ImGui.SliderFloat("Cave Exposure", ref _caveExposure, 0.1f, 1.0f);
        if (ImGui.IsItemEdited())
        {
            _map.GenMap(_dirtScale, _stoneScale, _caveScale, _caveExposure);
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