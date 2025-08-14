using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;

namespace weizinai.StardewValleyMod.HelpWanted.Manager;

public class VanillaQuestManager : QuestManager<VanillaQuestManager>
{
    private VanillaModConfig VanillaConfig => ModConfig.Instance.VanillaConfig;

    public void InitVanillaQuestList()
    {
        if (!this.CheckDayAvailable()) return;

        if (ModConfig.Instance.ShowQuestGenerationTooltip)
        {
            Logger.NoIconHUDMessage(I18n.UI_GenerateVanillaQuest_Tooltip());
        }

        var maxQuests = this.VanillaConfig.MaxQuests;
        var quest = this.GenerateVanillaQuest();
        int tries = 0, i = 0;
        var npcNames = new HashSet<string>(maxQuests);

        while (i < maxQuests && quest != null)
        {
            var npc = this.GetNPCFromQuest(quest);

            if (npc == null)
            {
                Logger.Error("Failed to retrieve NPC information for the quest; vanilla quest generation has been terminated.");

                break;
            }

            if (!this.CheckNPCAvailable(npcNames, npc))
            {
                if (++tries > 3)
                {
                    i++;
                    tries = 0;
                }
            }
            else
            {
                i++;
                tries = 0;
                npcNames.Add(npc.Name);
                this.QuestList.Add(this.GetQuestData(npc, quest));
                Logger.Debug($"Vanilla quest #{this.QuestList.Count} generated: {this.GetQuestType(quest)} - {npc.Name}");
            }

            if (i < maxQuests) quest = this.GenerateVanillaQuest();
        }
    }

    private bool CheckDayAvailable()
    {
        var showTooltip = ModConfig.Instance.ShowQuestGenerationTooltip;

        if (Game1.stats.DaysPlayed <= 1 && !this.VanillaConfig.QuestFirstDay)
        {
            if (showTooltip)
            {
                Logger.NoIconHUDMessage(I18n.UI_VanillaQuestFirstDay_Tooltip());
            }

            return false;
        }

        if ((Utility.isFestivalDay() || Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season)) && !this.VanillaConfig.QuestFestival)
        {
            if (showTooltip)
            {
                Logger.NoIconHUDMessage(I18n.UI_VanillaQuestFestival_Tooltip());
            }

            return false;
        }

        if (ModEntry.Random.NextDouble() >= this.VanillaConfig.DailyQuestChance)
        {
            if (showTooltip)
            {
                Logger.NoIconHUDMessage(I18n.UI_VanillaDailyQuest_Tooltip());
            }

            return false;
        }

        return true;
    }

    private bool CheckNPCAvailable(HashSet<string> npcNames, NPC npc)
    {
        var npcName = npc.Name;

        var oneQuestPerVillager = this.VanillaConfig.OneQuestPerVillager && npcNames.Contains(npcName);
        var excludeMaxHeartsNPC = this.VanillaConfig.ExcludeMaxHeartsNPC
                                  && Game1.player.tryGetFriendshipLevelForNPC(npcName) >= Utility.GetMaximumHeartsForCharacter(npc) * 250;
        var excludeNPCList = this.VanillaConfig.ExcludeNPCList.Contains(npc.displayName);

        var available = !oneQuestPerVillager && !excludeMaxHeartsNPC && !excludeNPCList;

        if (!available)
        {
            var reasons = new List<string>();
            if (oneQuestPerVillager) reasons.Add("Existing");
            if (excludeMaxHeartsNPC) reasons.Add("Maximum Hearts");
            if (excludeNPCList) reasons.Add("Excluded");
            Logger.Trace($"{npcName} cannot be assigned as a quest target due to: {string.Join(";", reasons)}");
        }

        return available;
    }

    private Quest? GenerateVanillaQuest()
    {
        var randomDouble = ModEntry.Random.NextDouble();
        var slayMonsterQuest = MineShaft.lowestLevelReached > 0 && Game1.stats.DaysPlayed > 5U;
        var questTypes = new List<(float weight, Func<Quest> createQuest)>
        {
            (this.VanillaConfig.ResourceCollectionQuestConfig.Weight, () => new ResourceCollectionQuest()),
            (slayMonsterQuest ? this.VanillaConfig.SlayMonsterQuestConfig.Weight : 0, () => new SlayMonsterQuest()),
            (this.VanillaConfig.FishingQuestConfig.Weight, () => new FishingQuest()),
            (this.VanillaConfig.ItemDeliveryQuestConfig.Weight, () => new ItemDeliveryQuest())
        };

        var currentWeight = 0f;
        var totalWeight = this.VanillaConfig.ResourceCollectionQuestConfig.Weight
                          + (slayMonsterQuest ? this.VanillaConfig.SlayMonsterQuestConfig.Weight : 0)
                          + this.VanillaConfig.FishingQuestConfig.Weight
                          + this.VanillaConfig.ItemDeliveryQuestConfig.Weight;

        foreach (var (weight, createQuest) in questTypes)
        {
            currentWeight += weight;

            if (randomDouble < currentWeight / totalWeight)
            {
                var quest = createQuest();
                quest.dailyQuest.Value = true;
                quest.accepted.Value = true;
                quest.canBeCancelled.Value = true;
                quest.reloadDescription();
                quest.reloadObjective();

                return quest;
            }
        }

        Logger.Error("Vanilla quest generation failed.");

        return null;
    }
}