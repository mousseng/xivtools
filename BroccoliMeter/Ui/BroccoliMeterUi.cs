namespace BroccoliMeter.Ui;

using BroccoliMeter.Parser;
using ImGuiNET;

public sealed class BroccoliMeterUi : IDisposable
{
    private bool isVisible;
    
    private BroccoliParser BroccoliParser { get; }
    
    public BroccoliMeterUi(BroccoliParser parser)
    {
        this.BroccoliParser = parser;
    }

    public void Draw()
    {
        if (!this.isVisible) return;

        ImGui.Begin("Broccoli Meter", ref this.isVisible);
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
