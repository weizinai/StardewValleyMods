using System.Collections.Generic;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public class RSVItemDeliveryQuestBuilder : QuestBuilder<ItemDeliveryQuest>
{
    private static readonly List<string> QuestLibrary = new()
    {
        "72861000", "72861001", "72861002", "72861003", "72861004", "72861005", "72861009", "72861010", "72861011", "72861012",
        "72861013", "72861016", "72861017", "72861018", "72861019", "72861020", "72861022", "72861024", "72861025", "72861026",
        "72861027", "72861028"
    };

    private readonly string[] rawQuest;

    public RSVItemDeliveryQuestBuilder(ItemDeliveryQuest quest) : base(quest)
    {
        this.Quest.daysLeft.Value = ModConfig.Instance.RSVConfig.ItemDeliveryQuestConfig.Days;

        var randomId = ModEntry.Random.ChooseFrom(QuestLibrary);
        this.rawQuest = StardewValley.Quests.Quest.GetRawQuestFields(randomId);
        this.Quest.id.Value = randomId;
    }

    protected override bool TrySetQuestTarget()
    {
        this.Quest.target.Value = ArgUtility.SplitBySpaceAndGet(this.rawQuest[4], 0);

        return true;
    }

    protected override void SetQuestTitle()
    {
        this.Quest.questTitle = this.rawQuest[1];
    }

    protected override void SetQuestItemId()
    {
        var itemId = ArgUtility.SplitBySpaceAndGet(this.rawQuest[4], 1);
        this.Quest.ItemId.Value = ItemRegistry.QualifyItemId(itemId) ?? itemId;
        this.Quest.number.Value = int.Parse(ArgUtility.SplitBySpaceAndGet(this.rawQuest[4], 2, "1"));
    }

    protected override void SetQuestMoneyReward()
    {
        this.Quest.moneyReward.Value = int.Parse(this.rawQuest[6]);

        var originalReward = this.Quest.moneyReward.Value;
        this.Quest.moneyReward.Value = (int)(originalReward * ModConfig.Instance.RSVConfig.ItemDeliveryQuestConfig.RewardMultiplier);
        Logger.Trace($"The RSV item delivery quest reward has been adjusted from [{originalReward}] to [{this.Quest.moneyReward.Value}].");
    }

    protected override void SetQuestDescription()
    {
        this.Quest.questDescription = this.rawQuest[2];
    }

    protected override void SetQuestDialogue()
    {
        this.Quest.targetMessage = this.rawQuest[9];
    }

    protected override void SetQuestObjective()
    {
        this.Quest.currentObjective = this.rawQuest[3];
    }
}