using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Helper;
using weizinai.StardewValleyMod.PiCore.Extension;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class ClearCrystalHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var tool = ToolHelper.GetTool<MeleeWeapon>(this.Config.AutoClearCrystal.FindToolFromInventory);
        if (tool is null) return;

        this.ForEachTile(this.Config.AutoClearCrystal.Range, tile =>
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.IsIceCrystal() == true)
            {
                if (obj.performToolAction(tool))
                {
                    location.removeObject(tile, false);
                }
            }
            return true;
        });
    }
}