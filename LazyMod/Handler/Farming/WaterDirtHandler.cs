using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class WaterDirtHandler : BaseAutomationHandler
{
    public WaterDirtHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var wateringCan = ToolHelper.GetTool<WateringCan>(this.config.AutoWaterDirt.FindToolFromInventory);

        if (wateringCan is null)
        {
            return;
        }

        this.ForEachTile(this.config.AutoWaterDirt.Range, tile =>
        {
            if (player.Stamina <= this.config.AutoWaterDirt.StopStamina || wateringCan.WaterLeft <= 0)
            {
                return false;
            }

            location.terrainFeatures.TryGetValue(tile, out var tileFeature);

            if (tileFeature is HoeDirt hoeDirt && hoeDirt.state.Value == HoeDirt.dry)
            {
                if (hoeDirt.crop == null && this.config.WaterOnlyWhenCrop)
                {
                    return true;
                }

                this.UseToolOnTile(location, player, wateringCan, tile);

                if (player.ShouldHandleAnimationSound())
                {
                    player.playNearbySoundLocal("wateringCan");
                }
            }

            return true;
        });
    }
}
