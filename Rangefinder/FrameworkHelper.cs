namespace Rangefinder;

using System;
using System.Runtime.InteropServices;
using Dalamud.Game;
using Dalamud.Game.Gui;
using FFXIVClientStructs.FFXIV.Component.GUI;

public static unsafe class FrameworkHelper
{
    private delegate IntPtr GameAlloc(ulong size, IntPtr unk, IntPtr allocator, IntPtr alignment);
    private delegate IntPtr GetGameAllocator();

    private static GameAlloc _gameAlloc;
    private static GetGameAllocator _getGameAllocator;

    private static GameGui Gui { get; set; }
    private static IntPtr PlayerStaticAddress { get; set; }

    public static void Initialize(SigScanner scanner, GameGui gui)
    {
        Gui = gui;
        
        var gameAllocPtr = scanner.ScanText("E8 ?? ?? ?? ?? 49 83 CF FF 4C 8B F0");
        var getGameAllocatorPtr = scanner.ScanText("E8 ?? ?? ?? ?? 8B 75 08");
        
        PlayerStaticAddress = scanner.GetStaticAddressFromSig("8B D7 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 0F B7 E8");

        _gameAlloc = Marshal.GetDelegateForFunctionPointer<GameAlloc>(gameAllocPtr);
        _getGameAllocator = Marshal.GetDelegateForFunctionPointer<GetGameAllocator>(getGameAllocatorPtr);
    }

    public static AtkUnitBase* GetUnitBase(string name, int index = 1)
    {
        return (AtkUnitBase*) Gui.GetAddonByName(name, index);
    }

    public static IntPtr Alloc(ulong size)
    {
        if (_gameAlloc == null || _getGameAllocator == null) return IntPtr.Zero;
        return _gameAlloc(size, IntPtr.Zero, _getGameAllocator(), IntPtr.Zero);
    }
}
