using System.Collections.Generic;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public class RSVLostItemQuestBuilder : QuestBuilder<LostItemQuest>
{
    private static readonly List<string> QuestLibrary = new() { "72861008", "72861023", "72861029" };

    private readonly string[] rawQuest;

    public RSVLostItemQuestBuilder(LostItemQuest quest) : base(quest)
    {
        this.Quest.daysLeft.Value = ModConfig.Instance.RSVConfig.FishingQuestConfig.Days;

        var randomId = ModEntry.Random.ChooseFrom(QuestLibrary);
        this.rawQuest = StardewValley.Quests.Quest.GetRawQuestFields(randomId);
        this.Quest.id.Value = randomId;
    }

    protected override bool TrySetQuestTarget()
    {
        this.Quest.npcName.Value = ArgUtility.SplitBySpaceAndGet(this.rawQuest[4], 0);

        return true;
    }

    protected override void SetQuestTitle()
    {
        this.Quest.questTitle = this.rawQuest[1];
    }

    protected override void SetQuestItemId()
    {
        var rawField = ArgUtility.SplitBySpace(this.rawQuest[4]);

        var itemId = rawField[1];
        this.Quest.ItemId.Value = ItemRegistry.QualifyItemId(itemId);
        this.Quest.locationOfItem.Value = rawField[2];
        this.Quest.tileX.Value = int.Parse(rawField[3]);
        this.Quest.tileY.Value = int.Parse(rawField[4]);
    }

    protected override void SetQuestMoneyReward()
    {
        this.Quest.moneyReward.Value = int.Parse(this.rawQuest[6]);

        var originalReward = this.Quest.moneyReward.Value;
        this.Quest.moneyReward.Value = (int)(originalReward * ModConfig.Instance.RSVConfig.LostItemQuestConfig.RewardMultiplier);
        Logger.Trace($"The RSV lost item quest reward has been adjusted from [{originalReward}] to [{this.Quest.moneyReward.Value}].");
    }

    protected override void SetQuestDescription()
    {
        this.Quest.questDescription = this.rawQuest[2];
    }

    protected override void SetQuestDialogue()
    {
        // ignore
    }

    protected override void SetQuestObjective()
    {
        this.Quest.currentObjective = this.rawQuest[3];
    }
}