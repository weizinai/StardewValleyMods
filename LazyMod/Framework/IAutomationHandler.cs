using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Framework;

internal interface IAutomationHandler
{
    public bool IsEnable();
    
    public void Apply(Item? item, Farmer player, GameLocation location);
}