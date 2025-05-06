using System.Linq;
using StardewValley;

namespace weizinai.StardewValleyMod.HelpWanted.Model;

public class ResourceConfig
{
    public const int MiningSkill = 0;
    public const int ForagingSkill = 1;

    private readonly int reward;

    private readonly int baseValue;
    private readonly int skill;
    private readonly float factor;
    private readonly int minRandom;
    private readonly int maxRandom;
    private readonly float multiplier;
    private readonly int mod;

    private int HighestLevel => this.skill == MiningSkill ? this.HighestMiningLevel : this.HighestForagingLevel;
    private int HighestMiningLevel => Game1.getAllFarmers().Select(farmer => farmer.MiningLevel).Max();
    private int HighestForagingLevel => Game1.getAllFarmers().Select(farmer => farmer.ForagingLevel).Max();

    public ResourceConfig(int reward, int baseValue, int skill, float factor, int minRandom, int maxRandom, float multiplier, int mod)
    {
        this.reward = reward;
        this.baseValue = baseValue;
        this.skill = skill;
        this.factor = factor;
        this.minRandom = minRandom;
        this.maxRandom = maxRandom;
        this.multiplier = multiplier;
        this.mod = mod;
    }

    public int GetRandomNumber()
    {
        var randomInt = ModEntry.Random.Next(this.minRandom, this.maxRandom);

        var number = this.baseValue + (int)(this.HighestLevel * this.factor) + randomInt * 2;
        number = (int)(number * this.multiplier);
        number -= number % this.mod;

        return number;
    }

    public int GetReward()
    {
        return this.reward;
    }
}