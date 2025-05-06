using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ClearCrystalHandler : BaseAutomationHandler
{
    public ClearCrystalHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var tool = ToolHelper.GetTool<MeleeWeapon>(this.Config.AutoClearCrystal.FindToolFromInventory);
        if (tool is null) return;

        this.ForEachTile(this.Config.AutoClearCrystal.Range, tile =>
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId is "(O)319" or "(O)320" or "(O)321")
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