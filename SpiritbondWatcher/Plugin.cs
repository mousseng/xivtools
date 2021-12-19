namespace SpiritbondWatcher;

using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.IoC;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

public class Plugin : IDalamudPlugin
{
    public string Name => "Spiritbond Watcher";
    
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

        this.CommandManager.AddHandler("/sbw", new CommandInfo(this.OnCommand));
        this.Client.TerritoryChanged += this.OnZoneChange;
    }

    private void OnZoneChange(object? sender, ushort e)
    {
        this.OnCommand("/sbw", "");
    }

    private void OnCommand(string cmd, string args)
    {
        Task.Run(() =>
        {
            var items =
                (from bondedItem in Inventory.GetBondedItems()
                    join item in this.Data.Excel.GetSheet<Item>()
                        on bondedItem equals item.RowId
                    select item.Name).ToList();

            if (items.Any())
            {
                this.Chat.PrintChat(new XivChatEntry
                {
                    Message = "Gear fully bonded: " + string.Join(", ", items)
                });
            }
        });
    }

    public void Dispose()
    {
        this.CommandManager.RemoveHandler("/sbw");
        this.Client.TerritoryChanged -= this.OnZoneChange;
        this.PluginInterface.Dispose();
    }
}
