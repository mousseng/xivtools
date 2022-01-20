namespace Rangefinder;

using System.Numerics;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Component.GUI;

/// <summary>
/// Displays the precise distance to a target within the
/// game UI proper. Shamelessly ripped from SimpleTweaks:
/// https://github.com/Caraxi/SimpleTweaksPlugin
/// </summary>
public sealed unsafe class RangefinderUi : IDisposable
{
    private ClientState ClientState { get; }
    private TargetManager TargetManager { get; }

    private static Vector2 Position = new(0);
    private static Vector2 FocusPosition = new(0);
    private const byte FontSize = 14;
        
    public RangefinderUi(
        ClientState clientState,
        TargetManager targetManager)
    {
        this.ClientState = clientState;
        this.TargetManager = targetManager;
    }

    public void Dispose()
    {
        this.DrawGameUi(true);
    }

    public void DrawGameUi(bool reset = false)
    {
        var target = this.TargetManager.SoftTarget ?? this.TargetManager.Target;
        if (target != null || reset)
        {
            var ui = GameHelper.GetUnitBase("_TargetInfo", 1);
            if (ui != null && (ui->IsVisible || reset))
            {
                this.UpdateMainTarget(ui, target, reset);
            }
                
            var splitUi = GameHelper.GetUnitBase("_TargetInfoMainTarget", 1);
            if (splitUi != null && (splitUi->IsVisible || reset))
            {
                this.UpdateMainTargetSplit(splitUi, target, reset);
            }
        }
            
        if (this.TargetManager.FocusTarget != null || reset)
        {
            var ui = GameHelper.GetUnitBase("_FocusTargetInfo", 1);
            if (ui != null && (ui->IsVisible || reset))
            {
                this.UpdateFocusTarget(ui, this.TargetManager.FocusTarget, reset);
            }
        }
    }
        
    private void UpdateMainTarget(AtkUnitBase* unitBase, GameObject target, bool reset)
    {
        if (unitBase == null || unitBase->UldManager.NodeList == null || unitBase->UldManager.NodeListCount < 40)
        {
            return;
        }
        
        var gauge = (AtkComponentNode*) unitBase->UldManager.NodeList[36];
        var textNode = (AtkTextNode*) unitBase->UldManager.NodeList[39];
        GameHelper.SetSize(unitBase->UldManager.NodeList[37], reset ? 44 : 0, reset ? 20 : 0);

        this.UpdateGaugeBar(
            gauge,
            textNode,
            target,
            Position,
            reset);
    }
    
    private void UpdateFocusTarget(AtkUnitBase* unitBase, GameObject target, bool reset)
    {
        // if (Config.NoFocus)
        // {
        //     reset = true;
        // }
        
        if (unitBase == null || unitBase->UldManager.NodeList == null || unitBase->UldManager.NodeListCount < 11)
        {
            return;
        }
        
        var gauge = (AtkComponentNode*) unitBase->UldManager.NodeList[2];
        var textNode = (AtkTextNode*) unitBase->UldManager.NodeList[10];

        this.UpdateGaugeBar(
            gauge,
            textNode,
            target,
            FocusPosition,
            reset);
    }
    
    private void UpdateMainTargetSplit(AtkUnitBase* unitBase, GameObject target, bool reset)
    {
        if (unitBase == null || unitBase->UldManager.NodeList == null || unitBase->UldManager.NodeListCount < 9)
        {
            return;
        }
        
        var gauge = (AtkComponentNode*) unitBase->UldManager.NodeList[5];
        var textNode = (AtkTextNode*) unitBase->UldManager.NodeList[8];
        GameHelper.SetSize(unitBase->UldManager.NodeList[6], reset ? 44 : 0, reset ? 20 : 0);

        this.UpdateGaugeBar(
            gauge,
            textNode,
            target,
            Position,
            reset);
    } 
    
    private void UpdateGaugeBar(AtkComponentNode* gauge, AtkTextNode* cloneTextNode, GameObject target, Vector2 positionOffset, bool reset)
    {
        if (gauge == null || (ushort)gauge->AtkResNode.Type < 1000)
        {
            return;
        }
        
        AtkTextNode* textNode = null;
    
        for (var i = 5; i < gauge->Component->UldManager.NodeListCount; i++)
        {
            var node = gauge->Component->UldManager.NodeList[i];
            if (node->Type == NodeType.Text && node->NodeID == CustomNodes.Range)
            {
                textNode = (AtkTextNode*) node;
                break;
            }
        }

        if (textNode == null && reset) return; // Nothing to clean
    
        if (textNode == null)
        {
            textNode = GameHelper.CloneNode(cloneTextNode);
            textNode->AtkResNode.NodeID = CustomNodes.Range;
            var newStrPtr = GameHelper.Alloc(512);
            textNode->NodeText.StringPtr = (byte*) newStrPtr;
            textNode->NodeText.BufSize = 512;
            textNode->SetText("");
            GameHelper.ExpandNodeList(gauge, 1);
            gauge->Component->UldManager.NodeList[gauge->Component->UldManager.NodeListCount++] = (AtkResNode*) textNode;
    
            var nextNode = gauge->Component->UldManager.RootNode;
            while (nextNode->PrevSiblingNode != null)
            {
                nextNode = nextNode->PrevSiblingNode;
            }
            
            textNode->AtkResNode.ParentNode = (AtkResNode*) gauge;
            textNode->AtkResNode.ChildNode = null;
            textNode->AtkResNode.PrevSiblingNode = null;
            textNode->AtkResNode.NextSiblingNode = nextNode;
            nextNode->PrevSiblingNode = (AtkResNode*) textNode;
        }
    
        if (reset)
        {
            GameHelper.Hide(textNode);
            return;
        }
    
        textNode->AlignmentFontType = (byte)AlignmentType.BottomRight;
        
        GameHelper.SetPosition(textNode, positionOffset.X, positionOffset.Y);
        GameHelper.SetSize(textNode, gauge->AtkResNode.Width - 5, gauge->AtkResNode.Height);
        GameHelper.Show(textNode);
        textNode->TextColor = cloneTextNode->TextColor;
        textNode->EdgeColor = cloneTextNode->EdgeColor;
        textNode->FontSize = FontSize; // cloneTextNode->FontSize;
        
        
        var player = this.ClientState.LocalPlayer;
        if (player != null && player.ObjectId != target.ObjectId && target is Character)
        {
            var src = player.Position;
            var dst = target.Position;
        
            var xzDist = Math.Sqrt(Math.Pow(dst.X - src.X, 2) + Math.Pow(dst.Z - src.Z, 2)) - target.HitboxRadius;
            var distanceText = xzDist.ToString("F1") + "y";
            
            textNode->SetText(distanceText);
        }
        else
        {
            textNode->SetText(string.Empty);
        }
    }
}

public static class CustomNodes
{
    public const int
        TargetHp             = SimpleTweaksNodeBase + 1,
        SlideCastMarker      = SimpleTweaksNodeBase + 2,
        TimeUntilGpMax       = SimpleTweaksNodeBase + 3,
        ComboTimer           = SimpleTweaksNodeBase + 4,
        PartyListStatusTimer = SimpleTweaksNodeBase + 5,
        InventoryGil         = SimpleTweaksNodeBase + 6,
        GearPositionsBg      = SimpleTweaksNodeBase + 7, // and 8
        Range                = SimpleTweaksNodeBase + 9,
        SimpleTweaksNodeBase = 0x53540000;
}
