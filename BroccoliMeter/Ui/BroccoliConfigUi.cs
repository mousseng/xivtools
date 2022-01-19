namespace BroccoliMeter.Ui;

using System.Numerics;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;

public sealed class BroccoliConfigUi : IDisposable
{
    private bool isVisible;
    
    private DalamudPluginInterface Plugin { get; }
    
    public BroccoliConfigUi(DalamudPluginInterface plugin)
    {
        this.Plugin = plugin;
    }

    public void Draw()
    {
        if (!this.isVisible) return;

        ImGui.SetNextWindowSizeConstraints(new Vector2(700, 660) * ImGuiHelpers.GlobalScale, new Vector2(9999));
        ImGui.Begin("Broccoli Meter Configuration", ref this.isVisible);
        ImGui.End();
    }

    public void Toggle()
    {
        this.isVisible = !this.isVisible;
    }
    
    public void Dispose()
    {
    }
}
