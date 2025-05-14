using System.Linq;
using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class AnimalDoorHandler : BaseAutomationHandler, IAutomationHandlerWithDayChanged
{
    public override void Apply(Item? item, Farmer player, GameLocation location) { }

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