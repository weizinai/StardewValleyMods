namespace weizinai.StardewValleyMod.HelpWanted.Framework;

public class BaseQuestConfig
{
    public float Weight;
    public float RewardMultiplier;
    public int Days;

    public BaseQuestConfig(float weight, float rewardMultiplier, int days)
    {
        this.Weight = weight;
        this.RewardMultiplier = rewardMultiplier;
        this.Days = days;
    }
}