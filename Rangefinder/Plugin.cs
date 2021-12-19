namespace Rangefinder;

using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.IoC;
using Dalamud.Plugin;

public class Plugin : IDalamudPlugin
{
    public string Name => "Rangefinder";
    
    private DalamudPluginInterface PluginInterface { get; init; }
    private CommandManager CommandManager { get; init; }
    private ChatGui Chat { get; init; }

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] CommandManager commandManager,
        [RequiredVersion("1.0")] ChatGui chat)
    {
        this.PluginInterface = pluginInterface;
        this.CommandManager = commandManager;
        this.Chat = chat;

        this.CommandManager.AddHandler("/rf", new CommandInfo(this.OnCommand)
        {
            // TODO
        });
    }

    private void OnCommand(string cmd, string args)
    {
        this.Chat.PrintChat(new XivChatEntry
        {
            Message = "Hello world from the plugin",
            Type = XivChatType.Debug,
        });
    }

    public void Dispose()
    {
        this.PluginInterface.Dispose();
        this.CommandManager.RemoveHandler("/rf");
    }
}
