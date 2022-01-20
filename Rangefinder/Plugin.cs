namespace Rangefinder;

using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;

public sealed class Plugin : IDalamudPlugin
{
    public string Name => "Rangefinder";
    
    private DalamudPluginInterface PluginInterface { get; }
    private RangefinderUi RangefinderUi { get; }
    private Framework Framework { get; }

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] SigScanner scanner,
        [RequiredVersion("1.0")] Framework framework,
        [RequiredVersion("1.0")] GameGui gui,
        [RequiredVersion("1.0")] ClientState clientState,
        [RequiredVersion("1.0")] TargetManager targetManager)
    {
        this.PluginInterface = pluginInterface;
        this.Framework = framework;
        
        FrameworkHelper.Initialize(scanner, gui);
        this.RangefinderUi = new(clientState, targetManager);

        this.Framework.Update += this.OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        this.RangefinderUi.DrawGameUi();
    }

    public void Dispose()
    {
        this.Framework.Update -= this.OnFrameworkUpdate;
        this.RangefinderUi.Dispose();
    }
}
