using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class PetPetHandler : BaseAutomationHandler
{
    public PetPetHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var pets = location.characters.OfType<Pet>().ToList();
        if (!pets.Any()) return;

        this.ForEachTile(this.Config.AutoPetPet.Range, tile =>
        {
            foreach (var pet in pets)
            {
                if (this.CanPetPet(tile, player, pet))
                {
                    pet.checkAction(player, location);
                    return true;
                }
            }
            return true;
        });
    }

    private bool CanPetPet(Vector2 tile, Farmer player, Pet pet)
    {
        return pet.GetBoundingBox().Intersects(this.GetTileBoundingBox(tile)) &&
               (!pet.lastPetDay.TryGetValue(player.UniqueMultiplayerID, out var lastPetDay) || lastPetDay != Game1.Date.TotalDays);
    }
}