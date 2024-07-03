using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ClearDeadCropHandler : BaseAutomationHandler
{
    public ClearDeadCropHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var scythe = ToolHelper.GetTool<MeleeWeapon>(this.Config.AutoClearDeadCrop.FindToolFromInventory);
        if (scythe is null) return;

        this.ForEachTile(this.Config.AutoClearDeadCrop.Range, tile =>
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: not null } hoeDirt)
            {
                var crop = hoeDirt.crop;
                if (crop.dead.Value)
                {
                    hoeDirt.performToolAction(scythe, 0, tile);
                }
            }
            
            return true;
        });
    }
}