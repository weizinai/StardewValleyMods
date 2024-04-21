using StardewValley;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

public class AutoFood : Automate
{
    private readonly ModConfig config;
    private readonly Dictionary<SObject, string?> foodData = new();

    public AutoFood(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item)
    {
        if (tool is FishingRod fishingRod && (fishingRod.isReeling || fishingRod.isFishing || fishingRod.pullingOutOfWater)) return;

        FindFoodFromInventory(player);
        if (!foodData.Any()) return;

        // 自动吃食物-体力
        if (config.AutoEatFoodForStamina) AutoEatFoodForStamina(player);
        // 自动吃食物-生命值
        if (config.AutoEatFoodForHealth) AutoEatFoodForHealth(player);
        // 自动吃食物-Buff
        if (config.AutoEatFoodForBuff) AutoEatFoodForBuff(player);
        // 自动喝饮料-Buff
        if (config.AutoDrinkForBuff) AutoDrinkFoodForBuff(player);
        
    }

    // 自动吃食物-体力
    private void AutoEatFoodForStamina(Farmer player)
    {
        if (player.Stamina > player.MaxStamina * config.AutoEatFoodStaminaRate) return;

        if (!config.IntelligentFoodSelectionForStamina)
        {
            EatFirstFood(player, foodData.Keys.First());
            return;
        }

        var food = foodData.Keys.OrderBy(food => (food.Price / food.Edibility, -food.Stack)).First();
        EatFirstFood(player, food);
    }

    // 自动吃食物-生命值
    private void AutoEatFoodForHealth(Farmer player)
    {
        if (player.health > player.maxHealth * config.AutoEatFoodHealthRate) return;

        if (!config.IntelligentFoodSelectionForHealth)
        {
            EatFirstFood(player, foodData.Keys.First());
            return;
        }

        var food = foodData.Keys.OrderBy(food => (food.Price / food.Edibility, -food.Stack)).First();
        EatFirstFood(player, food);
    }

    // 自动吃食物-Buff
    private void AutoEatFoodForBuff(Farmer player)
    {
        var buffs = player.buffs.AppliedBuffs.Values.ToList();
        if (buffs.Any(buff => buff.id is "food")) return;
        
        var foodList = new List<SObject>();
        foreach (var (food, foodBuffId) in foodData) if (foodBuffId is "food") foodList.Add(food);
        if (!foodList.Any()) return;
        EatFirstFood(player, foodList.First());
    }
    
    
    // 自动喝饮料-Buff
    private void AutoDrinkFoodForBuff(Farmer player)
    {
        var buffs = player.buffs.AppliedBuffs.Values.ToList();
        var foodList = new List<SObject>();
        foreach (var (food, foodBuffId) in foodData) if (foodBuffId is "drink") foodList.Add(food);
        if (!foodList.Any()) return;
        
        if (!buffs.Any(buff => buff.id is "drink"))
        {
            EatFirstFood(player, foodList.First());
        }
    }

    private void FindFoodFromInventory(Farmer player)
    {
        foodData.Clear();
        foreach (var item in player.Items)
        {
            if (item is not SObject { Edibility: > 0 } obj) continue;
            var buffs = obj.GetFoodOrDrinkBuffs().ToList();
            if (!buffs.Any())
            {
                foodData.TryAdd(obj, null);
                continue;
            }

            if (buffs.Any(buff => buff.id == "food" && buff.effects.Speed.Value > 0))
            {
                foodData.TryAdd(obj, "food");
                continue;
            }

            if (buffs.Any(buff => buff.id == "drink" && buff.effects.Speed.Value > 0))
            {
                foodData.TryAdd(obj, "drink");
            }
        }
    }

    private bool CheckFoodOverrideStamina(SObject food)
    {
        return food.GetFoodOrDrinkBuffs().Any(buff => buff.effects.MaxStamina.Value > 0);
    }

    private void EatFirstFood(Farmer player, SObject food)
    {
        if (player.isEating) return;
        var direction = player.FacingDirection;
        player.eatObject(food, CheckFoodOverrideStamina(food));
        player.FacingDirection = direction;
        ConsumeItem(player, food);
    }
}