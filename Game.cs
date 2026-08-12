namespace Rayterra;

using Raylib_cs;
using ImGuiNET;
using rlImGui_cs;
using Core;

public class Rayterra : Game
{
    override protected string WindowTitle => "Rayterra";

    private Map _map = null!;

    private Player _player = null!;

    override protected void Init()
    {
        _map = new();
        _player = new(_map);

        _manager.AddEntity(_player);

        _map.GenMap(_dirtScale, _stoneScale, _caveScale, _caveExposure);
    }

    override protected void Render()
    {
        Raylib.ClearBackground(Color.SkyBlue);

        Raylib.BeginMode2D(_player.Camera.RaylibCamera);

        _map.Render(_player.Camera);
        using (new ProfilerScope("EntityRender")) { _manager.Render(); }

        Raylib.EndMode2D();
    }

    private float _dirtScale = 0.008f;
    private float _stoneScale = 0.03f;
    private float _caveScale = 13.2f;
    private float _caveExposure = 0.55f;

    override protected void RenderDebugUI()
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

        ImGui.Text($"Updating time: {Profiler.GetProfile("Update").time} ms");
        ImGui.Text($"Entity Updating time: {Profiler.GetProfile("EntityUpdate").time} ms");

        ImGui.NewLine();

        ImGui.Text($"Entity Rendering time: {Profiler.GetProfile("EntityRender").time} ms");
        ImGui.Text($"Rendering time: {Profiler.GetProfile("Render").time} ms");
        ImGui.Text($"Debug UI time: {Profiler.GetProfile("DebugUI").time} ms");
        ImGui.Text($"Present time: {Profiler.GetProfile("Present").time} ms");

        rlImGui.End();
    }

}