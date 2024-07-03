using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class PetPetHandler : BaseAutomationHandler
{
    public PetPetHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        var pets = location.characters.OfType<Pet>().ToList();
        if (!pets.Any()) return;

        var grid = this.GetTileGrid(this.Config.AutoPetAnimal.Range);

        foreach (var tile in grid)
        {
            foreach (var pet in pets)
            {
                if (this.CanPetPet(tile, player, pet))
                {
                    pet.checkAction(player, location);
                }
            }
        }
    }

    private bool CanPetPet(Vector2 tile, Farmer player, Pet pet)
    {
        return pet.GetBoundingBox().Intersects(this.GetTileBoundingBox(tile)) &&
               (!pet.lastPetDay.TryGetValue(player.UniqueMultiplayerID, out var lastPetDay) || lastPetDay != Game1.Date.TotalDays);
    }
}