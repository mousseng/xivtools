namespace BroccoliMeter;

using BroccoliMeter.Ui;
using BroccoliMeter.Parser;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Network;
using Dalamud.IoC;
using Dalamud.Plugin;

public sealed class BroccoliPlugin : IDalamudPlugin
{
    public string Name => "Broccoli Meter";
    
    private DalamudPluginInterface PluginInterface { get; }
    private BroccoliConfig Config { get; }
    private BroccoliUi BroccoliUi { get; }
    private BroccoliParser Parser { get; }

    public BroccoliPlugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] ObjectTable entities,
        [RequiredVersion("1.0")] PartyList party,
        [RequiredVersion("1.0")] GameNetwork network)
    {
        this.PluginInterface = pluginInterface;

        this.Parser = new(entities, party, network);
        this.Config = pluginInterface.GetPluginConfig() as BroccoliConfig ?? new();
        
        this.BroccoliUi = new(this.PluginInterface, this.Parser);
        this.PluginInterface.UiBuilder.Draw += this.BroccoliUi.Draw;
        this.PluginInterface.UiBuilder.OpenConfigUi += this.BroccoliUi.OpenConfig;
    }

    public void Dispose()
    {
        this.BroccoliUi.Dispose();
        this.Parser.Dispose();
    }
}
