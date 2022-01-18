namespace SpiritbondWatcher;

using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;

public sealed class Plugin : IDalamudPlugin
{
    public string Name => "Spiritbond Watcher";
    private const string Command = "/sbw";
    
    private DalamudPluginInterface PluginInterface { get; }
    private CommandManager CommandManager { get; }
    private ClientState Client { get; }
    private DataManager Data { get; }
    private ChatGui Chat { get; }

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] CommandManager commandManager,
        [RequiredVersion("1.0")] ClientState client,
        [RequiredVersion("1.0")] DataManager data,
        [RequiredVersion("1.0")] ChatGui chat)
    {
        this.PluginInterface = pluginInterface;
        this.CommandManager = commandManager;
        this.Client = client;
        this.Data = data;
        this.Chat = chat;

        this.CommandManager.AddHandler(Command, new CommandInfo(this.OnCommand));
        this.Client.TerritoryChanged += this.OnZoneChange;
    }

    private void OnZoneChange(object? sender, ushort e)
    {
        this.OnCommand(Command, "");
    }

    private void OnCommand(string cmd, string args)
    {
        Task.Run(() => GearChecker.CheckGear(this.Data, this.Chat));
    }

    public void Dispose()
    {
        this.CommandManager.RemoveHandler(Command);
        this.Client.TerritoryChanged -= this.OnZoneChange;
    }
}
