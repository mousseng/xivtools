namespace Rangefinder;

using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;

public static unsafe class UiHelper
{
    private static void Hide(AtkResNode* node)
    {
        node->Flags &= ~0x10;
        node->Flags_2 |= 0x1;
    }

    private static void Show(AtkResNode* node)
    {
        node->Flags |= 0x10;
        node->Flags_2 |= 0x1;
    }

    public static void SetSize(AtkResNode* node, int? width, int? height)
    {
        if (width != null && width >= ushort.MinValue && width <= ushort.MaxValue) node->Width = (ushort) width.Value;
        if (height != null && height >= ushort.MinValue && height <= ushort.MaxValue) node->Height = (ushort) height.Value;
        node->Flags_2 |= 0x1;
    }

    private static void SetPosition(AtkResNode* node, float? x, float? y)
    {
        if (x != null) node->X = x.Value;
        if (y != null) node->Y = y.Value;
        node->Flags_2 |= 0x1;
    }

    public static void ExpandNodeList(AtkComponentNode* componentNode, ushort addSize)
    {
        var newNodeList = ExpandNodeList(componentNode->Component->UldManager.NodeList, componentNode->Component->UldManager.NodeListCount, (ushort) (componentNode->Component->UldManager.NodeListCount + addSize));
        componentNode->Component->UldManager.NodeList = newNodeList;
    }

    private static AtkResNode** ExpandNodeList(AtkResNode** originalList, ushort originalSize, ushort newSize = 0)
    {
        if (newSize <= originalSize) newSize = (ushort)(originalSize + 1);
        var oldListPtr = new IntPtr(originalList);
        var newListPtr = FrameworkHelper.Alloc((ulong)((newSize + 1) * 8));
        var clone = new IntPtr[originalSize];
        Marshal.Copy(oldListPtr, clone, 0, originalSize);
        Marshal.Copy(clone, 0, newListPtr, originalSize);
        return (AtkResNode**)(newListPtr);
    }

    private static AtkResNode* CloneNode(AtkResNode* original)
    {
        var size = original->Type switch
        {
            NodeType.Res => sizeof(AtkResNode),
            NodeType.Image => sizeof(AtkImageNode),
            NodeType.Text => sizeof(AtkTextNode),
            NodeType.NineGrid => sizeof(AtkNineGridNode),
            NodeType.Counter => sizeof(AtkCounterNode),
            NodeType.Collision => sizeof(AtkCollisionNode),
            _ => throw new Exception("Unsupported Type: " + original->Type)
        };

        var allocation = FrameworkHelper.Alloc((ulong)size);
        var bytes = new byte[size];
        Marshal.Copy(new IntPtr(original), bytes, 0, bytes.Length);
        Marshal.Copy(bytes, 0, allocation, bytes.Length);

        var newNode = (AtkResNode*)allocation;
        newNode->ParentNode = null;
        newNode->ChildNode = null;
        newNode->ChildCount = 0;
        newNode->PrevSiblingNode = null;
        newNode->NextSiblingNode = null;
        return newNode;
    }
    
    public static void Hide<T>(T* node) where T : unmanaged => Hide((AtkResNode*)node);
    public static void Show<T>(T* node) where T : unmanaged => Show((AtkResNode*)node);
    public static void SetSize<T>(T* node, int? w, int? h) where T : unmanaged => SetSize((AtkResNode*) node, w, h);
    public static void SetPosition<T>(T* node, float? x, float? y) where T : unmanaged => SetPosition((AtkResNode*) node, x, y);
    public static T* CloneNode<T>(T* original) where T : unmanaged => (T*) CloneNode((AtkResNode*) original);
}
