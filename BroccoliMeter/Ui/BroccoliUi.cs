namespace BroccoliMeter.Ui;

using BroccoliMeter.Parser;
using Dalamud.Plugin;

public sealed class BroccoliUi : IDisposable
{
    private BroccoliConfigUi Config { get; }
    private BroccoliMeterUi Meter { get; }
    
    public BroccoliUi(
        DalamudPluginInterface plugin,
        BroccoliParser parser)
    {
        this.Config = new BroccoliConfigUi(plugin);
        this.Meter = new BroccoliMeterUi(parser);
    }

    public void Draw()
    {
        this.Meter.Draw();
        this.Config.Draw();
    }

    public void OpenConfig()
    {
        this.Config.Toggle();
    }
    
    public void Dispose()
    {
        this.Config.Dispose();
        this.Meter.Dispose();
    }
}
