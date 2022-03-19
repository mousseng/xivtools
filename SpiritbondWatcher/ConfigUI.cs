namespace SpiritbondWatcher;

using Dalamud.Interface.Components;
using ImGuiNET;
using System;
using System.Numerics;

class ConfigUI : IDisposable
{
    private Config configuration;

    public const ImGuiWindowFlags Flags = ImGuiWindowFlags.NoResize |
                                          ImGuiWindowFlags.NoCollapse |
                                          ImGuiWindowFlags.NoScrollbar |
                                          ImGuiWindowFlags.NoScrollWithMouse;

    private bool visible = false;
    public bool Visible
    {
        get { return this.visible; }
        set { this.visible = value; }
    }

    public ConfigUI(Config configuration)
    {
        this.configuration = configuration;
    }

    public void Draw()
    {
        if (!visible)
        {
            return;
        }

        ImGui.SetNextWindowSize(new Vector2(232, 75), ImGuiCond.Always);
        if (ImGui.Begin("Spiritbond Watcher", ref this.visible, Flags))
        {
            bool lineByLineValue = this.configuration.BondedGearDisplayLineByLine;
            if (ImGui.Checkbox("Display gear line by line", ref lineByLineValue))
            {
                if (this.configuration.BondedGearDisplayLineByLine != lineByLineValue)
                {
                    this.configuration.BondedGearDisplayLineByLine = lineByLineValue;
                    this.configuration.Save();
                }
            }
            ImGuiComponents.HelpMarker("Display bonded gear line by line");
        }
        ImGui.End();
    }

    public void Dispose() { }
}