using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Framework;

internal interface IAutomationHandler
{
    public void Apply(Item item, Farmer player, GameLocation location);
}