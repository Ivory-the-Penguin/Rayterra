namespace Dara;

using Entity;

using Raylib_cs;
using rlImGui_cs;

public abstract class Game
{
    virtual protected int WindowWidth => 1280;
    virtual protected int WindowHeight => 720;
    virtual protected string WindowTitle => "Game";
    virtual protected int Fps => 60;

    protected float deltaTime = 0;

    protected EntityManager _manager = new();

    public Game()
    {
        InitWindow();
        LoadTextures();

        Init();
    }


    virtual protected void InitWindow()
    {
        Raylib.InitWindow(WindowWidth, WindowHeight, WindowTitle);
        Raylib.SetTargetFPS(Fps);
        Raylib.ToggleFullscreen();

        rlImGui.Setup(true);
    }

    virtual protected void LoadTextures() { }

    virtual protected void Init() { }

    virtual protected void Update() { }

    virtual protected void Render() { }

    abstract protected void RenderDebugUI();

    private void Cleanup()
    {
        rlImGui.Shutdown();
        Assets.Cleanup();
        Raylib.CloseWindow();
    }

    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            deltaTime = Raylib.GetFrameTime();

            using (new ProfilerScope("EntityUpdate")) { _manager.Update(deltaTime); }

            using (new ProfilerScope("Update")) { Update(); }

            Raylib.BeginDrawing();

            using (new ProfilerScope("Render")) { Render(); }

#if DEBUG
            using (new ProfilerScope("DebugUI")) { RenderDebugUI(); }
#endif

            using (new ProfilerScope("Present")) { Raylib.EndDrawing(); }
        }
        Cleanup();
    }
}