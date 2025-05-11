using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public interface IAutomationHandler
{
    public bool IsEnable();

    public void Apply(Item? item, Farmer player, GameLocation location);
}