using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class PetAnimalHandler : BaseAutomationHandler
{
    public PetAnimalHandler(ModConfig config) : base(config) { }

    public override void Apply(Farmer player, GameLocation location)
    {
        var animals = location.animals.Values;
        if (!animals.Any()) return;

        var grid = this.GetTileGrid(this.Config.AutoPetAnimal.Range);

        foreach (var tile in grid)
        {
            foreach (var animal in animals)
            {
                if (this.CanPetAnimal(tile, animal))
                {
                    this.PetAnimal(player, animal);
                }
            }
        }
    }

    private bool CanPetAnimal(Vector2 tile, FarmAnimal animal)
    {
        return animal.GetBoundingBox().Intersects(this.GetTileBoundingBox(tile)) &&
               !animal.wasPet.Value &&
               (animal.isMoving() || Game1.timeOfDay < 1900) &&
               !animal.Name.StartsWith("DH.MEEP.SpawnedAnimal_");
    }

    private void PetAnimal(Farmer player, FarmAnimal animal)
    {
        animal.wasPet.Value = true;

        // 好感度和心情逻辑
        var data = animal.GetAnimalData();
        var happinessDrain = data?.HappinessDrain ?? 0;
        animal.friendshipTowardFarmer.Value = animal.wasAutoPet.Value
            ? Math.Min(1000, animal.friendshipTowardFarmer.Value + 7)
            : Math.Min(1000, animal.friendshipTowardFarmer.Value + 15);
        animal.happiness.Value = Math.Min(255, animal.happiness.Value + Math.Max(5, 30 + happinessDrain));
        if (data is { ProfessionForHappinessBoost: >= 0 } && player.professions.Contains(data.ProfessionForHappinessBoost))
        {
            animal.friendshipTowardFarmer.Value = Math.Min(1000, animal.friendshipTowardFarmer.Value + 15);
            animal.happiness.Value = Math.Min(255, animal.happiness.Value + Math.Max(5, 30 + happinessDrain));
        }

        // 标签逻辑
        var emoteIndex = animal.wasAutoPet.Value ? 20 : 32;
        animal.doEmote(animal.moodMessage.Value == 4 ? 12 : emoteIndex);

        // 声音逻辑
        animal.makeSound();

        // 经验逻辑
        player.gainExperience(0, 5);
    }
}