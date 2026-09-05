using System.Collections.Generic;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public class RSVSlayMonsterQuestBuilder : QuestBuilder<SlayMonsterQuest>
{
    private static readonly List<string> QuestLibrary = new() { "72861006", "72861007", "72861014", "72861015", "72861021" };

    private readonly string[] rawQuest;

    public RSVSlayMonsterQuestBuilder(SlayMonsterQuest quest) : base(quest)
    {
        this.quest.daysLeft.Value = ModConfig.Instance.RSVConfig.SlayMonsterQuestConfig.Days;

        var randomId = ModEntry.Random.ChooseFrom(QuestLibrary);
        this.rawQuest = Quest.GetRawQuestFields(randomId);
        this.quest.id.Value = randomId;
    }

    protected override bool TrySetQuestTarget()
    {
        this.quest.target.Value = ArgUtility.SplitBySpaceAndGet(this.rawQuest[4], 2);

        return true;
    }

    protected override void SetQuestTitle()
    {
        this.quest.questTitle = this.rawQuest[1];
    }

    protected override void SetQuestItemId()
    {
        this.quest.monsterName.Value = ArgUtility.SplitBySpaceAndGet(this.rawQuest[4], 0).Replace("_", " ");
        this.quest.monster.Value = new Monster
        {
            Name = this.quest.monsterName.Value
        };
        this.quest.numberToKill.Value = int.Parse(ArgUtility.SplitBySpaceAndGet(this.rawQuest[4], 1));
    }

    protected override void SetQuestMoneyReward()
    {
        this.quest.reward.Value = int.Parse(this.rawQuest[6]);

        var originalReward = this.quest.reward.Value;
        this.quest.reward.Value = (int)(originalReward * ModConfig.Instance.RSVConfig.SlayMonsterQuestConfig.RewardMultiplier);
        Logger<ModEntry>.Trace($"The RSV slay monster quest reward has been adjusted from [{originalReward}] to [{this.quest.reward.Value}].");
    }

    protected override void SetQuestDescription()
    {
        this.quest.questDescription = this.rawQuest[2];
    }

    protected override void SetQuestDialogue()
    {
        this.quest.targetMessage = this.rawQuest[9];
    }

    protected override void SetQuestObjective()
    {
        this.quest.currentObjective = this.rawQuest[3];
    }
}
