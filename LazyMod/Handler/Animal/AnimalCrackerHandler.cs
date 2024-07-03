using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class AnimalCrackerHandler : BaseAutomationHandler
{
    public AnimalCrackerHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        if (item.QualifiedItemId != "(O)GoldenAnimalCracker") return;

        var animals = location.animals.Values;
        
        this.ForEachTile(this.Config.AutoFeedAnimalCracker.Range, tile =>
        {
            foreach (var animal in animals)
            {
                if (this.CanFeedAnimalCracker(tile, animal))
                {
                    this.FeedAnimalCracker(player, animal);
                    return true;
                }
            }
            
            return true;
        });
    }

    private bool CanFeedAnimalCracker(Vector2 tile, FarmAnimal animal)
    {
        return animal.GetBoundingBox().Intersects(this.GetTileBoundingBox(tile)) &&
               !animal.hasEatenAnimalCracker.Value &&
               (animal.GetAnimalData()?.CanEatGoldenCrackers ?? false);
    }

    private void FeedAnimalCracker(Farmer player, FarmAnimal animal)
    {
        animal.hasEatenAnimalCracker.Value = true;
        Game1.playSound("give_gift");
        animal.doEmote(56);
        player.reduceActiveItemByOne();
    }
}