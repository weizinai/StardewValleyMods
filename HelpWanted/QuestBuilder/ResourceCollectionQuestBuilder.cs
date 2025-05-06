using System.Collections.Generic;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Model;
using static weizinai.StardewValleyMod.HelpWanted.Helper.PathStringHelper;
using static weizinai.StardewValleyMod.HelpWanted.Repository.ItemRepository;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public class ResourceCollectionQuestBuilder : QuestBuilder<ResourceCollectionQuest>
{
    private static readonly Dictionary<string, ResourceConfig> ItemConfig = new()
    {
        [CopperOre] = new ResourceConfig(
            10, 20, ResourceConfig.MiningSkill, 2f, -2, 4, 1f, 5
        ),
        [IronOre] = new ResourceConfig(
            15, 15, ResourceConfig.MiningSkill, 1f, -1, 3, 0.75f, 5
        ),
        [Coal] = new ResourceConfig(
            20, 10, ResourceConfig.MiningSkill, 1f, -1, 3, 0.75f, 5
        ),
        [GoldOre] = new ResourceConfig(
            30, 8, ResourceConfig.MiningSkill, 0.5f, -1, 1, 0.75f, 2
        ),
        [IridiumOre] = new ResourceConfig(
            50, 5, ResourceConfig.MiningSkill, 0.5f, -1, 1, 0.75f, 2
        ),
        [CinderShard] = new ResourceConfig(
            50, 5, ResourceConfig.MiningSkill, 0.5f, -1, 1, 0.75f, 2
        ),
        [Wood] = new ResourceConfig(
            8, 25, ResourceConfig.ForagingSkill, 1f, -3, 3, 1f, 5
        ),
        [Stone] = new ResourceConfig(
            8, 25, ResourceConfig.MiningSkill, 1f, -3, 3, 1f, 5
        ),
        [Hardwood] = new ResourceConfig(
            30, 10, ResourceConfig.ForagingSkill, 1f, -3, 3, 1f, 5
        ),
    };

    private readonly int randomIndex;

    private Item item = null!;

    public ResourceCollectionQuestBuilder(ResourceCollectionQuest quest) : base(quest)
    {
        quest.daysLeft.Value = ModConfig.Instance.VanillaConfig.ResourceCollectionQuestConfig.Days;

        this.randomIndex = ModEntry.Random.Next(4);
    }

    protected override bool TrySetQuestTarget()
    {
        if (this.Quest.target.Value != null || Game1.gameMode == 6)
        {
            Logger.Trace($"Target for the current resource collection quest has been set to {this.Quest.target.Value}.");
            return false;
        }

        this.Quest.target.Value = ModEntry.Random.NextBool() ? "Clint" : "Robin";

        return true;
    }

    protected override void SetQuestTitle()
    {
        this.Quest.questTitle = Game1.content.LoadString(GetPathString("R", 13640));
    }

    protected override void SetQuestItemId()
    {
        var random = ModEntry.Random;
        var moreQuest = ModConfig.Instance.VanillaConfig.MoreResourceCollectionQuest;
        var possibleItems = new List<string>(4);

        switch (this.Quest.target.Value)
        {
            case "Clint":
            {
                possibleItems.AddRange(new[] { CopperOre, IronOre, Coal });
                if (Utility.GetAllPlayerDeepestMineLevel() > 40) possibleItems.Add(GoldOre);
                if (!moreQuest) break;
                if (Game1.player.mailReceived.Contains("ccVault")) possibleItems.Add(IridiumOre);
                if (Game1.player.mailReceived.Contains("willyHours")) possibleItems.Add(CinderShard);
                break;
            }
            case "Robin":
            {
                possibleItems.AddRange(new[] { Wood, Stone });
                if (!moreQuest) break;
                if (Game1.player.locationsVisited.Contains("Woods")) possibleItems.Add(Hardwood);
                break;
            }
        }

        this.Quest.ItemId.Value = random.ChooseFrom(possibleItems);
        this.Quest.number.Value = ItemConfig.TryGetValue(this.Quest.ItemId.Value, out var config) ? config.GetRandomNumber() : 1;
        this.item = ItemRegistry.Create(this.Quest.ItemId.Value);
    }

    protected override void SetQuestMoneyReward()
    {
        this.Quest.reward.Value = this.Quest.number.Value * (ItemConfig.TryGetValue(this.Quest.ItemId.Value, out var config) ? config.GetReward() : 0);

        var originalReward = this.Quest.reward.Value;
        this.Quest.reward.Value = (int)(originalReward * ModConfig.Instance.VanillaConfig.ResourceCollectionQuestConfig.RewardMultiplier);
        Logger.Trace($"The vanilla resource collection quest reward has been adjusted from [{originalReward}] to [{this.Quest.reward.Value}].");
    }

    protected override void SetQuestDescription()
    {
        this.Quest.parts.Clear();

        if (this.Quest.target.Value == "Robin")
        {
            this.Quest.parts.Add(new DescriptionElement(
                GetPathString("R", 13674),
                this.Quest.number.Value,
                this.item
            ));
        }
        else
        {
            this.Quest.parts.Add(new DescriptionElement(
                GetPathString("R", 13647),
                this.Quest.number.Value,
                this.item,
                new DescriptionElement(GetPathString("R", new[] { 13649, 13650, 13651, 13652 }[this.randomIndex]))
            ));
        }

        this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13607), this.Quest.reward.Value));
        this.Quest.parts.Add(this.Quest.target.Value == "Clint" ? GetPathString("R", 13688) : "");
    }

    protected override void SetQuestDialogue()
    {
        var random = ModEntry.Random;
        this.Quest.dialogueparts.Clear();

        if (this.Quest.target.Value == "Robin")
        {
            this.Quest.dialogueparts.Add(new DescriptionElement(
                GetPathString("R", 13677),
                this.Quest.ItemId.Value == Wood
                    ? new DescriptionElement(GetPathString("R", 13678))
                    : new DescriptionElement(GetPathString("R", 13679))
            ));
            this.Quest.dialogueparts.Add(GetPathString("R", 13681, 13682, 13683));
        }
        else
        {
            if (this.randomIndex == 3)
            {
                this.Quest.dialogueparts.Add(GetPathString("R", 13655));
                this.Quest.dialogueparts.Add(GetPathString("R", 13656, 13657, 13658));
                this.Quest.dialogueparts.Add(GetPathString("R", 13659));
            }
            else
            {
                this.Quest.dialogueparts.Add(GetPathString("R", 13662));
                this.Quest.dialogueparts.Add(GetPathString("R", 13656, 13657, 13658));
                this.Quest.dialogueparts.Add(random.NextBool()
                    ? new DescriptionElement(GetPathString("R", 13667), new DescriptionElement(GetPathString("R", 13668, 13669, 13670)))
                    : new DescriptionElement(GetPathString("R", 13672)));
                this.Quest.dialogueparts.Add(GetPathString("R", 13673));
            }
        }
    }

    protected override void SetQuestObjective()
    {
        this.Quest.objective.Value = new DescriptionElement(
            GetPathString("R", 13691),
            "0",
            this.Quest.number.Value,
            this.item
        );
    }
}