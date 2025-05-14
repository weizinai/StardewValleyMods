using static weizinai.StardewValleyMod.PiCore.Constant.SItem;

namespace weizinai.StardewValleyMod.PiCore.Extension;

public static class SObjectExtensions
{
    public static bool IsIceCrystal(this SObject obj)
    {
        return obj.QualifiedItemId is IceCrystalNode1 or IceCrystalNode2 or IceCrystalNode3;
    }
}