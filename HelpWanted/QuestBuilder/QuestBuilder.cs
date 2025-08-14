using StardewValley.Quests;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public abstract class QuestBuilder<T> where T : Quest
{
    protected readonly T Quest;

    protected abstract bool TrySetQuestTarget();

    protected abstract void SetQuestTitle();

    protected abstract void SetQuestItemId();

    protected abstract void SetQuestMoneyReward();

    protected abstract void SetQuestDescription();

    protected abstract void SetQuestDialogue();

    protected abstract void SetQuestObjective();

    protected QuestBuilder(T quest)
    {
        this.Quest = quest;
    }

    public virtual void BuildQuest()
    {
        if (!this.TrySetQuestTarget()) return;

        this.SetQuestTitle();
        this.SetQuestItemId();
        this.SetQuestMoneyReward();
        this.SetQuestDescription();
        this.SetQuestDialogue();
        this.SetQuestObjective();
    }
}