using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class ShearsAnimalHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var shears = ToolHelper.GetTool<Shears>(this.Config.AutoShearsAnimal.FindToolFromInventory);

        if (shears == null) return;

        var animals = location.animals.Values;

        this.ForEachTile(this.Config.AutoShearsAnimal.Range, tile =>
        {
            if (player.Stamina <= this.Config.AutoShearsAnimal.StopStamina || player.freeSpotsInInventory() < 1) return false;

            var animal = this.GetBestHarvestableFarmAnimal(shears, tile, animals);
            if (animal is not null)
            {
                shears.animal = animal;
                this.UseToolOnTile(location, player, shears, tile);
            }

            return true;
        });
    }
}