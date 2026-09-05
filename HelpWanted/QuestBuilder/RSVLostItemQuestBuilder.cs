using System.Collections.Generic;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public class RSVLostItemQuestBuilder : QuestBuilder<LostItemQuest>
{
    private static readonly List<string> QuestLibrary = new() { "72861008", "72861023", "72861029" };

    private readonly string[] rawQuest;

    public RSVLostItemQuestBuilder(LostItemQuest quest) : base(quest)
    {
        this.quest.daysLeft.Value = ModConfig.Instance.RSVConfig.FishingQuestConfig.Days;

        var randomId = ModEntry.Random.ChooseFrom(QuestLibrary);
        this.rawQuest = Quest.GetRawQuestFields(randomId);
        this.quest.id.Value = randomId;
    }

    protected override bool TrySetQuestTarget()
    {
        this.quest.npcName.Value = ArgUtility.SplitBySpaceAndGet(this.rawQuest[4], 0);

        return true;
    }

    protected override void SetQuestTitle()
    {
        this.quest.questTitle = this.rawQuest[1];
    }

    protected override void SetQuestItemId()
    {
        var rawField = ArgUtility.SplitBySpace(this.rawQuest[4]);

        var itemId = rawField[1];
        this.quest.ItemId.Value = ItemRegistry.QualifyItemId(itemId);
        this.quest.locationOfItem.Value = rawField[2];
        this.quest.tileX.Value = int.Parse(rawField[3]);
        this.quest.tileY.Value = int.Parse(rawField[4]);
    }

    protected override void SetQuestMoneyReward()
    {
        this.quest.moneyReward.Value = int.Parse(this.rawQuest[6]);

        var originalReward = this.quest.moneyReward.Value;
        this.quest.moneyReward.Value = (int)(originalReward * ModConfig.Instance.RSVConfig.LostItemQuestConfig.RewardMultiplier);
        Logger<ModEntry>.Trace($"The RSV lost item quest reward has been adjusted from [{originalReward}] to [{this.quest.moneyReward.Value}].");
    }

    protected override void SetQuestDescription()
    {
        this.quest.questDescription = this.rawQuest[2];
    }

    protected override void SetQuestDialogue()
    {
        // ignore
    }

    protected override void SetQuestObjective()
    {
        this.quest.currentObjective = this.rawQuest[3];
    }
}
