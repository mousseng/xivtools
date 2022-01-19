namespace Rangefinder;

using System.Numerics;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Plugin;
using ImGuiNET;

public sealed class RangefinderUi : IDisposable
{
    private DalamudPluginInterface Plugin { get; }
    private ClientState ClientState { get; }
    private TargetManager TargetManager { get; }
        
    public RangefinderUi(
        DalamudPluginInterface plugin,
        ClientState clientState,
        TargetManager targetManager)
    {
        this.Plugin = plugin;
        this.ClientState = clientState;
        this.TargetManager = targetManager;
    }

    public void Draw()
    {
        // TODO: hide in cutscenes
        if (this.ClientState.LocalPlayer == null || this.TargetManager.Target == null)
        {
            return;
        }
        
        var windowFlags = ImGuiWindowFlags.NoResize
                        | ImGuiWindowFlags.NoInputs
                        | ImGuiWindowFlags.NoMove
                        | ImGuiWindowFlags.NoTitleBar
                        | ImGuiWindowFlags.AlwaysAutoResize;

        var initialPos = new Vector2(400, 400);
        var bgAlpha = 0.5f;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.SetNextWindowPos(initialPos, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowBgAlpha(bgAlpha);
        ImGui.Begin("Rangefinder", windowFlags);
        
        // if (this.resettingMonitorPos)
        // {
        //     ImGui.SetWindowPos(this.config.MonitorPosition);
        //     this.resettingMonitorPos = false;
        // }

        var src = this.ClientState.LocalPlayer.Position;
        var dst = this.TargetManager.Target.Position;
        
        var xyDist = Math.Sqrt(Math.Pow(dst.X - src.X, 2) + Math.Pow(dst.Y - src.Y, 2)) - this.TargetManager.Target.HitboxRadius;
        var distanceText = xyDist.ToString("F1") + "y";
        
        // TODO: color text for in/out of range based on job and target type
        //       e.g., RED for >25y from enemy while a caster
        //       e.g., RED for >3y from enemy while a melee
        //       e.g., RED for >30y from friendly while a healer
        var fontColor = new Vector4(255, 255, 255, 1);
        
        ImGui.TextColored(fontColor, distanceText);

        ImGui.End();
        ImGui.PopStyleVar();
    }
    
    public void Dispose()
    {
    }
}
