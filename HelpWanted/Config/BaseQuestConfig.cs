namespace weizinai.StardewValleyMod.HelpWanted.Config;

public class BaseQuestConfig
{
    public float Weight { get; set; }
    public float RewardMultiplier { get; set; }
    public int Days { get; set; }

    public BaseQuestConfig(float weight, float rewardMultiplier, int days)
    {
        this.Weight = weight;
        this.RewardMultiplier = rewardMultiplier;
        this.Days = days;
    }
}
