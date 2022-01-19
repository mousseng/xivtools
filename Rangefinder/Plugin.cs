namespace Rangefinder;

using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;

public sealed class Plugin : IDalamudPlugin
{
    public string Name => "Rangefinder";
    
    private DalamudPluginInterface PluginInterface { get; }
    private RangefinderUi RangefinderUi { get; }

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] ClientState clientState,
        [RequiredVersion("1.0")] TargetManager targetManager)
    {
        this.PluginInterface = pluginInterface;
        this.RangefinderUi = new(this.PluginInterface, clientState, targetManager);

        this.PluginInterface.UiBuilder.Draw += this.RangefinderUi.Draw;
    }

    public void Dispose()
    {
        this.PluginInterface.UiBuilder.Draw -= this.RangefinderUi.Draw;
    }
}
