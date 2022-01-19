namespace BroccoliMeter;

using Dalamud.Configuration;

[Serializable]
public class BroccoliConfig : IPluginConfiguration
{
    public int Version { get; set; } = 1;
}
