using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Constant;
using weizinai.StardewValleyMod.PiCore.Extension;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class AnimalCrackerHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (item?.QualifiedItemId == SItem.GoldenAnimalCracker)
        {
            var animals = location.animals.Values;

            this.ForEachTile(this.Config.AutoFeedAnimalCracker.Range, tile =>
            {
                foreach (var animal in animals)
                {
                    if (this.CanFeedAnimalCracker(tile, animal))
                    {
                        animal.EatGoldenAnimalCracker();
                        return true;
                    }
                }

                return true;
            });
        }
    }

    private bool CanFeedAnimalCracker(Vector2 tile, FarmAnimal animal)
    {
        return animal.GetBoundingBox().Intersects(this.GetTileBoundingBox(tile)) 
               && !animal.hasEatenAnimalCracker.Value 
               && (animal.GetAnimalData()?.CanEatGoldenCrackers ?? false);
    }
}