using System;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using static weizinai.StardewValleyMod.HelpWanted.Helper.PathStringHelper;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public class RSVFishingQuestBuilder : QuestBuilder<FishingQuest>
{
    private Item fish = null!;

    public RSVFishingQuestBuilder(FishingQuest quest) : base(quest)
    {
        this.Quest.daysLeft.Value = ModConfig.Instance.RSVConfig.FishingQuestConfig.Days;
    }

    protected override bool TrySetQuestTarget()
    {
        this.Quest.target.Value = "Carmen";

        return true;
    }

    protected override void SetQuestTitle()
    {
        this.Quest.questTitle = Game1.content.LoadString(GetPathString("F", 13227));
    }

    protected override void SetQuestItemId()
    {
        var random = ModEntry.Random;

        this.Quest.ItemId.Value = Game1.season switch
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
            _ => this.Quest.ItemId.Value
        };

        this.fish = ItemRegistry.Create(this.Quest.ItemId.Value);
        this.Quest.numberToFish.Value = (int)Math.Ceiling(200.0 / Math.Max(1, this.fish.salePrice())) + Game1.player.FishingLevel / 5;
    }

    protected override void SetQuestMoneyReward()
    {
        this.Quest.reward.Value = (int)(this.Quest.numberToFish.Value + 1.5) * this.fish.salePrice();

        var originalReward = this.Quest.reward.Value;
        this.Quest.reward.Value = (int)(originalReward * ModConfig.Instance.RSVConfig.FishingQuestConfig.RewardMultiplier);
        Logger.Trace($"The RSV fishing quest reward has been adjusted from [{originalReward}] to [{this.Quest.reward.Value}].");
    }

    protected override void SetQuestDescription()
    {
        this.Quest.parts.Clear();
        this.Quest.parts.Add(new DescriptionElement(
            "Strings\\StringsFromCSFiles:Carmen.FishingQuest.Description",
            this.fish.DisplayName,
            this.Quest.numberToFish.Value
        ));
        this.Quest.parts.Add(new DescriptionElement(GetPathString("F", 13274), this.Quest.reward.Value));
        this.Quest.parts.Add(GetPathString("F", 13275));
    }

    protected override void SetQuestDialogue()
    {
        this.Quest.dialogueparts.Clear();
        this.Quest.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:Carmen.FishingQuest.HandInDialogue", this.fish.DisplayName));
    }

    protected override void SetQuestObjective()
    {
        this.Quest.objective.Value = new DescriptionElement(
            GetPathString("F", 13244),
            0,
            this.Quest.numberToFish.Value,
            this.fish.DisplayName
        );
    }
}