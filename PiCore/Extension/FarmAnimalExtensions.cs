using StardewValley;

namespace weizinai.StardewValleyMod.PiCore.Extension;

public static class FarmAnimalExtensions
{
    // 代码来源：FarmAnimal.pet(Farmer who, bool is_auto_pet = false)
    public static void EatGoldenAnimalCracker(this FarmAnimal animal)
    {
        animal.hasEatenAnimalCracker.Value = true;
        Game1.playSound("give_gift");
        animal.doEmote(56);
        Game1.player.reduceActiveItemByOne();
    }
}