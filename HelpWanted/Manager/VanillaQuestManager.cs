using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.HelpWanted.Manager;

public class VanillaQuestManager : QuestManager<VanillaQuestManager>
{
    private VanillaModConfig vanillaConfig => ModConfig.Instance.VanillaConfig;

    public void InitVanillaQuestList()
    {
        if (!this.CheckDayAvailable()) return;

        if (ModConfig.Instance.ShowQuestGenerationTooltip) HudLogger.NoIconHUDMessage(I18n.UI_GenerateVanillaQuest_Tooltip());

        var maxQuests = this.vanillaConfig.MaxQuests;
        var quest = this.GenerateVanillaQuest();
        int tries = 0, i = 0;
        var npcNames = new HashSet<string>(maxQuests);

        while (i < maxQuests && quest != null)
        {
            var npc = this.GetNPCFromQuest(quest);

            if (npc == null)
            {
                Logger<ModEntry>.Error("Failed to retrieve NPC information for the quest; vanilla quest generation has been terminated.");

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
                Logger<ModEntry>.Debug($"Vanilla quest #{this.QuestList.Count} generated: {this.GetQuestType(quest)} - {npc.Name}");
            }

            if (i < maxQuests) quest = this.GenerateVanillaQuest();
        }
    }

    private bool CheckDayAvailable()
    {
        var showTooltip = ModConfig.Instance.ShowQuestGenerationTooltip;

        if (Game1.stats.DaysPlayed <= 1 && !this.vanillaConfig.QuestFirstDay)
        {
            if (showTooltip) HudLogger.NoIconHUDMessage(I18n.UI_VanillaQuestFirstDay_Tooltip());

            return false;
        }

        if ((Utility.isFestivalDay() || Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season)) && !this.vanillaConfig.QuestFestival)
        {
            if (showTooltip) HudLogger.NoIconHUDMessage(I18n.UI_VanillaQuestFestival_Tooltip());

            return false;
        }

        if (ModEntry.Random.NextDouble() >= this.vanillaConfig.DailyQuestChance)
        {
            if (showTooltip) HudLogger.NoIconHUDMessage(I18n.UI_VanillaDailyQuest_Tooltip());

            return false;
        }

        return true;
    }

    private bool CheckNPCAvailable(HashSet<string> npcNames, NPC npc)
    {
        var npcName = npc.Name;

        var oneQuestPerVillager = this.vanillaConfig.OneQuestPerVillager && npcNames.Contains(npcName);
        var excludeMaxHeartsNPC = this.vanillaConfig.ExcludeMaxHeartsNPC
                                  && Game1.player.tryGetFriendshipLevelForNPC(npcName) >= Utility.GetMaximumHeartsForCharacter(npc) * 250;
        var excludeNPCList = this.vanillaConfig.ExcludeNPCList.Contains(npc.displayName);

        var available = !oneQuestPerVillager && !excludeMaxHeartsNPC && !excludeNPCList;

        if (!available)
        {
            var reasons = new List<string>();

            if (oneQuestPerVillager) reasons.Add("Existing");

            if (excludeMaxHeartsNPC) reasons.Add("Maximum Hearts");

            if (excludeNPCList) reasons.Add("Excluded");

            Logger<ModEntry>.Trace($"{npcName} cannot be assigned as a quest target due to: {string.Join(";", reasons)}");
        }

        return available;
    }

    private Quest? GenerateVanillaQuest()
    {
        var randomDouble = ModEntry.Random.NextDouble();
        var slayMonsterQuest = MineShaft.lowestLevelReached > 0 && Game1.stats.DaysPlayed > 5U;
        var questTypes = new List<(float weight, Func<Quest> createQuest)>
        {
            (this.vanillaConfig.ResourceCollectionQuestConfig.Weight, () => new ResourceCollectionQuest()),
            (slayMonsterQuest ? this.vanillaConfig.SlayMonsterQuestConfig.Weight : 0, () => new SlayMonsterQuest()),
            (this.vanillaConfig.FishingQuestConfig.Weight, () => new FishingQuest()),
            (this.vanillaConfig.ItemDeliveryQuestConfig.Weight, () => new ItemDeliveryQuest())
        };

        var currentWeight = 0f;
        var totalWeight = this.vanillaConfig.ResourceCollectionQuestConfig.Weight
                          + (slayMonsterQuest ? this.vanillaConfig.SlayMonsterQuestConfig.Weight : 0)
                          + this.vanillaConfig.FishingQuestConfig.Weight
                          + this.vanillaConfig.ItemDeliveryQuestConfig.Weight;

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

        Logger<ModEntry>.Error("Vanilla quest generation failed.");

        return null;
    }
}
