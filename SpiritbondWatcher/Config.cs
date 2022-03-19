namespace SpiritbondWatcher;

using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

[Serializable]
public class Config : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool BondedGearDisplayLineByLine { get; set; } = false;

    [NonSerialized]
    private DalamudPluginInterface? pluginInterface;

    public void Initialize(DalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    public void Save()
    {
        this.pluginInterface!.SavePluginConfig(this);
    }
}
