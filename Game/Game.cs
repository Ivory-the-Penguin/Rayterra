using Rayterra.Core.Entity;

namespace Rayterra.Game;

using Raylib_cs;
using ImGuiNET;
using rlImGui_cs;

public class Game
{
    private const int WINDOW_WIDTH = 1280;
    private const int WINDOW_HEIGHT = 720;
    private const string WINDOW_TITLE = "Rayterra";
    private const int FPS = 60;

    private readonly EntityManager _manager = new();

    private void Init()
    {
        Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, WINDOW_TITLE);
        Raylib.SetTargetFPS(FPS);
        Raylib.ToggleFullscreen();

        rlImGui.Setup(true);
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
        Init();
        while (!Raylib.WindowShouldClose())
        {
            Update();
            Render();
        }
        Cleanup();
    }
}