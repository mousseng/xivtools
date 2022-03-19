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

    private DalamudPluginInterface PluginInterface { get; init; }
    private CommandManager CommandManager { get; init; }
    private ClientState Client { get; init; }
    private DataManager Data { get; init; }
    private ChatGui Chat { get; init; }
    private Config Config { get; init; }
    private ConfigUI ConfigUI { get; init; }

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

        this.Config = this.PluginInterface.GetPluginConfig() as Config ?? new Config();
        this.Config.Initialize(this.PluginInterface);
        this.ConfigUI = new ConfigUI(this.Config);

        this.CommandManager.AddHandler(Command, new CommandInfo(this.OnCommand)
        {
            HelpMessage = "Display bonded gear"
        });
        this.Client.TerritoryChanged += this.OnZoneChange;

        this.PluginInterface.UiBuilder.Draw += DrawUI;
        this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    private void OnZoneChange(object? sender, ushort e)
    {
        this.OnCommand(Command, "zone");
    }

    private void OnCommand(string cmd, string args)
    {
        Task.Run(() => GearChecker.CheckGear(this.Data, this.Chat, this.Config, args));
    }

    private void DrawUI()
    {
        this.ConfigUI.Draw();
    }

    private void DrawConfigUI()
    {
        this.ConfigUI.Visible = true;
    }

    public void Dispose()
    {
        this.CommandManager.RemoveHandler(Command);
        this.Client.TerritoryChanged -= this.OnZoneChange;
    }
}