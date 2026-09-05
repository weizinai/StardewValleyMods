using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ClearTilledDirtHandler : BaseAutomationHandler
{
    public ClearTilledDirtHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var pickaxe = ToolHelper.GetTool<Pickaxe>(this.config.AutoClearTilledDirt.FindToolFromInventory);

        if (pickaxe is null)
        {
            return;
        }

        this.ForEachTile(this.config.AutoClearTilledDirt.Range, tile =>
        {
            if (player.Stamina <= this.config.AutoClearTilledDirt.StopStamina)
            {
                return false;
            }

            location.terrainFeatures.TryGetValue(tile, out var tileFeature);

            if (tileFeature is HoeDirt { crop: null } hoeDirt && hoeDirt.state.Value == HoeDirt.dry)
            {
                this.UseToolOnTile(location, player, pickaxe, tile);
            }

            return true;
        });
    }
}
