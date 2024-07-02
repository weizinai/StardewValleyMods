using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Framework;

internal interface IAutomationHandler
{
    public void Apply(Farmer player, GameLocation location);
}