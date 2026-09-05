using System;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.PiCore.Logging;
using static weizinai.StardewValleyMod.HelpWanted.Helper.PathStringHelper;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public class RSVFishingQuestBuilder : QuestBuilder<FishingQuest>
{
    private Item fish = null!;

    public RSVFishingQuestBuilder(FishingQuest quest) : base(quest)
    {
        this.quest.daysLeft.Value = ModConfig.Instance.RSVConfig.FishingQuestConfig.Days;
    }

    protected override bool TrySetQuestTarget()
    {
        this.quest.target.Value = "Carmen";

        return true;
    }

    protected override void SetQuestTitle()
    {
        this.quest.questTitle = Game1.content.LoadString(GetPathString("F", 13227));
    }

    protected override void SetQuestItemId()
    {
        var random = ModEntry.Random;

        this.quest.ItemId.Value = Game1.season switch
        {
            Season.Spring => random.Choose<string>(
                "(O)Rafseazz.RSVCP_Cutthroat_Trout",
                "(O)Rafseazz.RSVCP_Ridgeside_Bass",
                "(O)Rafseazz.RSVCP_Ridge_Bluegill",
                "(O)Rafseazz.RSVCP_Caped_Tree_Frog",
                "(O)Rafseazz.RSVCP_Pebble_Back_Crab",
                "(O)Rafseazz.RSVCP_Harvester_Trout",
                "(O)Rafseazz.RSVCP_Mountain_Redbelly_Dace",
                "(O)Rafseazz.RSVCP_Mountain_Whitefish"
            ),
            Season.Summer => random.Choose<string>(
                "(O)Rafseazz.RSVCP_Cutthroat_Trout",
                "(O)Rafseazz.RSVCP_Ridgeside_Bass",
                "(O)Rafseazz.RSVCP_Caped_Tree_Frog",
                "(O)Rafseazz.RSVCP_Pebble_Back_Crab",
                "(O)Rafseazz.RSVCP_Skulpin_Fish",
                "(O)Rafseazz.RSVCP_Mountain_Redbelly_Dace",
                "(O)Rafseazz.RSVCP_Mountain_Whitefish"
            ),
            Season.Fall => random.Choose<string>(
                "(O)Rafseazz.RSVCP_Cutthroat_Trout",
                "(O)Rafseazz.RSVCP_Ridgeside_Bass",
                "(O)Rafseazz.RSVCP_Ridge_Bluegill",
                "(O)Rafseazz.RSVCP_Caped_Tree_Frog",
                "(O)Rafseazz.RSVCP_Pebble_Back_Crab",
                "(O)Rafseazz.RSVCP_Skulpin_Fish",
                "(O)Rafseazz.RSVCP_Harvester_Trout",
                "(O)Rafseazz.RSVCP_Mountain_Redbelly_Dace",
                "(O)Rafseazz.RSVCP_Mountain_Whitefish"
            ),
            Season.Winter => random.Choose<string>(
                "(O)Rafseazz.RSVCP_Ridgeside_Bass",
                "(O)Rafseazz.RSVCP_Ridge_Bluegill",
                "(O)Rafseazz.RSVCP_Skulpin_Fish",
                "(O)Rafseazz.RSVCP_Harvester_Trout",
                "(O)Rafseazz.RSVCP_Mountain_Redbelly_Dace",
                "(O)Rafseazz.RSVCP_Mountain_Whitefish"
            ),
            _ => this.quest.ItemId.Value
        };

        this.fish = ItemRegistry.Create(this.quest.ItemId.Value);
        this.quest.numberToFish.Value = (int)Math.Ceiling(200.0 / Math.Max(1, this.fish.salePrice())) + Game1.player.FishingLevel / 5;
    }

    protected override void SetQuestMoneyReward()
    {
        this.quest.reward.Value = (int)(this.quest.numberToFish.Value + 1.5) * this.fish.salePrice();

        var originalReward = this.quest.reward.Value;
        this.quest.reward.Value = (int)(originalReward * ModConfig.Instance.RSVConfig.FishingQuestConfig.RewardMultiplier);
        Logger<ModEntry>.Trace($"The RSV fishing quest reward has been adjusted from [{originalReward}] to [{this.quest.reward.Value}].");
    }

    protected override void SetQuestDescription()
    {
        this.quest.parts.Clear();
        this.quest.parts.Add(new DescriptionElement(
            "Strings\\StringsFromCSFiles:Carmen.FishingQuest.Description",
            this.fish.DisplayName,
            this.quest.numberToFish.Value
        ));
        this.quest.parts.Add(new DescriptionElement(GetPathString("F", 13274), this.quest.reward.Value));
        this.quest.parts.Add(GetPathString("F", 13275));
    }

    protected override void SetQuestDialogue()
    {
        this.quest.dialogueparts.Clear();
        this.quest.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:Carmen.FishingQuest.HandInDialogue", this.fish.DisplayName));
    }

    protected override void SetQuestObjective()
    {
        this.quest.objective.Value = new DescriptionElement(
            GetPathString("F", 13244),
            0,
            this.quest.numberToFish.Value,
            this.fish.DisplayName
        );
    }
}
