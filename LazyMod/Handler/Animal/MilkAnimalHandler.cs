using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class MilkAnimalHandler : BaseAutomationHandler
{
    public MilkAnimalHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        var milkPail = ToolHelper.GetTool<MilkPail>(this.Config.AutoMilkAnimal.FindToolFromInventory);
        if (milkPail is null) return;

        var animals = location.animals.Values;
        if (!animals.Any()) return;
        
        this.ForEachTile(this.Config.AutoMilkAnimal.Range, tile =>
        {
            if (player.freeSpotsInInventory() == 0) return false;
            if (player.Stamina <= this.Config.AutoMilkAnimal.StopStamina) return false;

            var animal = this.GetBestHarvestableFarmAnimal(milkPail, tile, animals);
            if (animal is not null)
            {
                milkPail.animal = animal;
                this.UseToolOnTile(location, player, milkPail, tile);
            }

            return true;
        });
    }
}