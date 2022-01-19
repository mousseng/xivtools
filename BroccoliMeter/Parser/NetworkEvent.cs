namespace BroccoliMeter.Parser;

using System.Runtime.InteropServices;
using Machina.FFXIV.Headers;

public class NetworkEvent
{
    private Type StructType { get; }
    private object Struct { get; }
    
    public NetworkEvent(ushort opcode, IntPtr packet)
    {
        this.Struct = Parse(opcode, packet);
        this.StructType = this.Struct.GetType();
    }

    private static object Parse(ushort opcode, IntPtr data)
    {
        return opcode switch
        {
            0x0188 => Deserialize<Server_StatusEffectList>(data),
            0x0293 => Deserialize<Server_StatusEffectList2>(data),
            0x0353 => Deserialize<Server_StatusEffectList3>(data),
            0x038f => Deserialize<Server_BossStatusEffectList>(data),
            0x033e => Deserialize<Server_ActionEffect1>(data),
            0x01f4 => Deserialize<Server_ActionEffect8>(data),
            0x01fa => Deserialize<Server_ActionEffect16>(data),
            0x0300 => Deserialize<Server_ActionEffect24>(data),
            0x03cd => Deserialize<Server_ActionEffect32>(data),
            0x0307 => Deserialize<Server_ActorCast>(data),
            0x0203 => Deserialize<Server_EffectResult>(data),
            0x0330 => Deserialize<Server_EffectResultBasic>(data),
            0x02cf => Deserialize<Server_ActorControl>(data),
            0x0096 => Deserialize<Server_ActorControlSelf>(data),
            0x0272 => Deserialize<Server_ActorControlTarget>(data),
            0x00f4 => Deserialize<Server_UpdateHpMpTp>(data),
            // 0x01e8 => Deserialize<Server_PlayerSpawn>(data),
            // 0x01d2 => Deserialize<Server_NpcSpawn>(data),
            // 0x0270 => Deserialize<Server_NpcSpawn2>(data),
            // 0x00db => Deserialize<Server_ActorMove>(data),
            // 0x0081 => Deserialize<Server_ActorSetPos>(data),
            0x022d => Deserialize<Server_ActorGauge>(data),
            0x0067 => Deserialize<Server_PresetWaymark>(data),
            0x00fd => Deserialize<Server_Waymark>(data),
            0x027a => Deserialize<Server_SystemLogMessage>(data),
            _ => throw new NotImplementedException(),
        };
    }
    
    private static T Deserialize<T>(IntPtr data) where T : struct
    {
        return Marshal.PtrToStructure<T>(data);
    }
}
