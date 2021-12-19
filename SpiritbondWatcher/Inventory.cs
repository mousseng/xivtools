namespace SpiritbondWatcher;

using FFXIVClientStructs.FFXIV.Client.Game;

public static unsafe class Inventory
{
    public static IEnumerable<uint> GetBondedItems(IEnumerable<InventoryType> inventoriesToSearch)
    {
        var bondedItems = new List<uint>();
        var manager = InventoryManager.Instance();

        foreach (var invType in inventoriesToSearch)
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
