using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

namespace weizinai.StardewValleyMod.HelpWanted.Manager;

public class RSVQuestManager : QuestManager<RSVQuestManager>
{
    private RSVModConfig RSVConfig => ModConfig.Instance.RSVConfig;

    public void InitRSVQuestList()
    {
        if (!this.CheckDayAvailable()) return;

        if (ModConfig.Instance.ShowQuestGenerationTooltip)
        {
            Logger.NoIconHUDMessage(I18n.UI_GenerateRSVQuest_Tooltip());
        }

        var maxQuests = this.RSVConfig.MaxQuests;
        var quest = this.GenerateRSVQuest();
        int tries = 0, i = 0;
        var questIds = new HashSet<string>();

        while (i < maxQuests && quest != null)
        {
            var npc = this.GetNPCFromQuest(quest);

            if (npc == null)
            {
                Logger.Error("Failed to retrieve NPC information for the quest; RSV quest generation has been terminated.");
                break;
            }

            if (!ModConfig.Instance.RSVConfig.AllowSameQuest
                && quest is not FishingQuest
                && (questIds.Contains(quest.id.Value) || Game1.player.hasQuest(quest.id.Value))
               )
            {
                if (++tries > 3)
                {
                    i++;
                    tries = 0;
                }
                Logger.Trace($"Duplicate RSV quest detected: ID {quest.id.Value} already exists. Regenerating new RSV quest.");
            }
            else
            {
                i++;
                tries = 0;
                questIds.Add(quest.id.Value);
                this.QuestList.Add(this.GetQuestData(npc, quest));
                Logger.Debug($"RSV quest #{this.QuestList.Count} generated: {this.GetQuestType(quest)} - {npc.Name}");
            }

            if (i < maxQuests) quest = this.GenerateRSVQuest();
        }
    }

    private bool CheckDayAvailable()
    {
        if (Game1.stats.DaysPlayed <= 1 && !this.RSVConfig.QuestFirstDay)
        {
            Logger.NoIconHUDMessage(I18n.UI_RSVQuestFirstDay_Tooltip());
            return false;
        }

        if ((Utility.isFestivalDay() || Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season)) && !this.RSVConfig.QuestFestival)
        {
            Logger.NoIconHUDMessage(I18n.UI_RSVQuestFestival_Tooltip());
            return false;
        }

        if (ModEntry.Random.NextDouble() >= this.RSVConfig.DailyQuestChance)
        {
            Logger.NoIconHUDMessage(I18n.UI_RSVDailyQuest_Tooltip());
            return false;
        }

        return true;
    }

    private Quest? GenerateRSVQuest()
    {
        var questTypes = new List<(float weight, Func<Quest> createQuest)>
        {
            (this.RSVConfig.ItemDeliveryQuestConfig.Weight, () => new ItemDeliveryQuest()),
            (this.RSVConfig.FishingQuestConfig.Weight, () => new FishingQuest()),
            (this.RSVConfig.SlayMonsterQuestConfig.Weight, () => new SlayMonsterQuest()),
            (this.RSVConfig.LostItemQuestConfig.Weight, () => new LostItemQuest())
        };

        var randomDouble = ModEntry.Random.NextDouble();
        var currentWeight = 0f;
        var totalWeight = this.RSVConfig.ItemDeliveryQuestConfig.Weight
                          + this.RSVConfig.FishingQuestConfig.Weight
                          + this.RSVConfig.SlayMonsterQuestConfig.Weight
                          + this.RSVConfig.LostItemQuestConfig.Weight;

        foreach (var (weight, createQuest) in questTypes)
        {
            currentWeight += weight;
            if (randomDouble < currentWeight / totalWeight)
            {
                var quest = createQuest();
                switch (quest)
                {
                    case ItemDeliveryQuest itemDeliveryQuest:
                    {
                        var builder = new RSVItemDeliveryQuestBuilder(itemDeliveryQuest);
                        builder.BuildQuest();
                        break;
                    }
                    case FishingQuest fishingQuest:
                    {
                        var builder = new RSVFishingQuestBuilder(fishingQuest);
                        builder.BuildQuest();
                        break;
                    }
                    case SlayMonsterQuest slayMonsterQuest:
                    {
                        var builder = new RSVSlayMonsterQuestBuilder(slayMonsterQuest);
                        builder.BuildQuest();
                        break;
                    }
                    case LostItemQuest lostItemQuest:
                    {
                        var builder = new RSVLostItemQuestBuilder(lostItemQuest);
                        builder.BuildQuest();
                        break;
                    }
                }

                quest.canBeCancelled.Value = true;
                return quest;
            }
        }

        Logger.Error("RSV quest generation failed.");
        return null;
    }
}