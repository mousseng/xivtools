namespace SpiritbondWatcher;

using Dalamud.Data;
using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;

public static class GearChecker
{
    private static readonly InventoryType[] InventoriesToSearch = {
        InventoryType.EquippedItems,
        InventoryType.ArmoryBody,
        InventoryType.ArmoryEar,
        InventoryType.ArmoryFeets,
        InventoryType.ArmoryHands,
        InventoryType.ArmoryHead,
        InventoryType.ArmoryLegs,
        InventoryType.ArmoryNeck,
        InventoryType.ArmoryRings,
        InventoryType.ArmoryWrist,
        InventoryType.ArmoryMainHand,
        InventoryType.ArmoryOffHand,
        // InventoryType.Inventory1,
        // InventoryType.Inventory2,
        // InventoryType.Inventory3,
        // InventoryType.Inventory4,
    };
    
    public static void CheckGear(DataManager data, ChatGui chat)
    {
        var items =
            (from bondedItem in Inventory.GetBondedItems(InventoriesToSearch)
                join item in data.Excel.GetSheet<Item>()
                    on bondedItem equals item.RowId
                select item.Name).ToList();

        if (items.Any())
        {
            chat.PrintChat(new XivChatEntry
            {
                Message = "Gear fully bonded: " + string.Join(", ", items)
            });
        }
    }
}
