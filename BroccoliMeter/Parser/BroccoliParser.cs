namespace BroccoliMeter.Parser;

using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Network;

public sealed class BroccoliParser : IDisposable
{
    private ObjectTable Entities { get; }
    private PartyList Party { get; }
    private GameNetwork Network { get; }
    private IList<string> Events { get; }
    
    public BroccoliParser(
        ObjectTable entities,
        PartyList party,
        GameNetwork network)
    {
        this.Entities = entities;
        this.Party = party;
        this.Network = network;
        
        this.Network.NetworkMessage += this.OnNetworkMessage;
    }

    public void Dispose()
    {
        this.Network.NetworkMessage -= this.OnNetworkMessage;
    }

    private void OnNetworkMessage(
        IntPtr data,
        ushort opcode,
        uint sourceActorId,
        uint targetActorId,
        NetworkMessageDirection direction)
    {
        if (direction == NetworkMessageDirection.ZoneUp) return;
        
        try
        {
            // the data pointer does not include the packet header,
            // which we don't actually care about BUT machina's
            // struct defs include them. so we move the pointer back
            // to the beginning of the header.
            var evt = new NetworkEvent(opcode, data - 0x20);
        }
        catch (NotImplementedException)
        {
        }
    }

    private void AddEvent()
    {
        this.Events.Add("");
    }
}
