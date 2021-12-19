namespace SpiritbondWatcher;

using FFXIVClientStructs.FFXIV.Client.Game;

public static unsafe class Inventory
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
    
    public static IEnumerable<uint> GetBondedItems()
    {
        var bondedItems = new List<uint>();
        var manager = InventoryManager.Instance();

        foreach (var invType in InventoriesToSearch)
        {
            var container = manager->GetInventoryContainer(invType);
            var size = container->Size;

            for (var i = 0; i < size; i++)
            {
                var item = container->GetInventorySlot(i);
                if (item->ItemID != 0 && item->Spiritbond >= 10000)
                {
                    bondedItems.Add(item->ItemID);
                }
            }
        }

        return bondedItems;
    } 
}
