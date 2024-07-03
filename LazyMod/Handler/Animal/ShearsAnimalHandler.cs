using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ShearsAnimalHandler : BaseAutomationHandler
{
    public ShearsAnimalHandler(ModConfig config) : base(config) { }

    public override void Apply(Farmer player, GameLocation location)
    {
        var shears = ToolHelper.GetTool<Shears>(this.Config.AutoShearsAnimal.FindToolFromInventory);
        if (shears is null) return;

        var animals = location.animals.Values;
        var grid = this.GetTileGrid(this.Config.AutoShearsAnimal.Range);

        foreach (var tile in grid)
        {
            if (player.Stamina <= this.Config.AutoShearsAnimal.StopStamina || player.freeSpotsInInventory() < 1) return;

            var animal = this.GetBestHarvestableFarmAnimal(shears, tile, animals);
            if (animal is null) continue;

            shears.animal = animal;
            this.UseToolOnTile(location, player, shears, tile);
        }
    }
}