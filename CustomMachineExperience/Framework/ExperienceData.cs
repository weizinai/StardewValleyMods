namespace weizinai.StardewValleyMod.CustomMachineExperience.Framework;

internal class ExperienceData
{
    public int FarmingExperience { get; set; }
    public int FishingExperience { get; set; }
    public int ForagingExperience { get; set; }
    public int MiningExperience { get; set; }
    public int CombatExperience { get; set; }

    public override string ToString() =>
        $"Farming {this.FarmingExperience} Fishing {this.FishingExperience} Foraging {this.ForagingExperience} Mining {this.MiningExperience} Combat {this.CombatExperience}";
}