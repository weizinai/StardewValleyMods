namespace weizinai.StardewValleyMod.HelpWanted.Model;

public class MonsterConfig
{
    private readonly string target;
    private readonly int reward;
    private readonly int minRandom;
    private readonly int maxRandom;
    private readonly int mod;

    public MonsterConfig(string target, int reward, int minRandom = 3, int maxRandom = 7, int mod = 1)
    {
        this.target = target;
        this.reward = reward;
        this.minRandom = minRandom;
        this.maxRandom = maxRandom;
        this.mod = mod;
    }

    public string GetTarget() => this.target;

    public int GetReward() => this.reward;

    public int GetRandomNumber()
    {
        var number = ModEntry.Random.Next(this.minRandom, this.maxRandom);
        number -= number % this.mod;

        return number;
    }
}