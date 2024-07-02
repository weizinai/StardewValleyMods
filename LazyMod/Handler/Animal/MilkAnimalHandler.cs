using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class MilkAnimalHandler : BaseAutomationHandler
{
    public MilkAnimalHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        var milkPail = ToolHelper.GetTool<MilkPail>(this.Config.AutoMilkAnimal.FindToolFromInventory);
        if (milkPail is null) return;

        var animals = location.animals.Values;
        if (!animals.Any()) return;

        var grid = this.GetTileGrid(this.Config.AutoMilkAnimal.Range);
        foreach (var tile in grid)
        {
            if (player.freeSpotsInInventory() == 0) break;
            if (player.Stamina <= this.Config.AutoMilkAnimal.StopStamina) break;
            
            var animal = this.GetBestHarvestableFarmAnimal(milkPail, tile, animals);
            if (animal is null) continue;
            
            milkPail.animal = animal;
            this.UseToolOnTile(location, player, milkPail, tile);
        }
    }
}