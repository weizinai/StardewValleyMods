using System.Collections.Generic;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public class RSVSlayMonsterQuestBuilder : QuestBuilder<SlayMonsterQuest>
{
    private static readonly List<string> QuestLibrary = new() { "72861006", "72861007", "72861014", "72861015", "72861021" };

    private readonly string[] rawQuest;

    public RSVSlayMonsterQuestBuilder(SlayMonsterQuest quest) : base(quest)
    {
        this.Quest.daysLeft.Value = ModConfig.Instance.RSVConfig.SlayMonsterQuestConfig.Days;

        var randomId = ModEntry.Random.ChooseFrom(QuestLibrary);
        this.rawQuest = StardewValley.Quests.Quest.GetRawQuestFields(randomId);
        this.Quest.id.Value = randomId;
    }

    protected override bool TrySetQuestTarget()
    {
        this.Quest.target.Value = ArgUtility.SplitBySpaceAndGet(this.rawQuest[4], 2);

        return true;
    }

    protected override void SetQuestTitle()
    {
        this.Quest.questTitle = this.rawQuest[1];
    }

    protected override void SetQuestItemId()
    {
        this.Quest.monsterName.Value = ArgUtility.SplitBySpaceAndGet(this.rawQuest[4], 0).Replace("_", " ");
        this.Quest.monster.Value = new Monster
        {
            Name = this.Quest.monsterName.Value
        };
        this.Quest.numberToKill.Value = int.Parse(ArgUtility.SplitBySpaceAndGet(this.rawQuest[4], 1));
    }

    protected override void SetQuestMoneyReward()
    {
        this.Quest.reward.Value = int.Parse(this.rawQuest[6]);

        var originalReward = this.Quest.reward.Value;
        this.Quest.reward.Value = (int)(originalReward * ModConfig.Instance.RSVConfig.SlayMonsterQuestConfig.RewardMultiplier);
        Logger.Trace($"The RSV slay monster quest reward has been adjusted from [{originalReward}] to [{this.Quest.reward.Value}].");
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