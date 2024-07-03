using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class AnimalDoorHandler : BaseAutomationHandler, IAutomationHandlerWithDayChanged
{
    public AnimalDoorHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location) { }

    public void OnDayStarted()
    {
        this.ToggleAnimalDoor(true);
    }

    public void OnDayEnding()
    {
        this.ToggleAnimalDoor(false);
    }

    private void ToggleAnimalDoor(bool isOpen)
    {
        if (isOpen && (Game1.isRaining || Game1.IsWinter)) return;

        var location = Game1.currentLocation;
        Utility.ForEachBuilding(building =>
        {
            if (building.animalDoor is not null && building.animalDoorOpen.Value != isOpen)
            {
                foreach (var animal in location.Animals.Values.Where(animal => !animal.IsHome && animal.home == building)) animal.warpHome();
                building.ToggleAnimalDoor(Game1.player);
            }
            return true;
        });
    }
}