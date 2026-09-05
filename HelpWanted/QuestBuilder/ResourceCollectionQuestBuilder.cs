using System.Collections.Generic;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Model;
using weizinai.StardewValleyMod.PiCore.Logging;
using static weizinai.StardewValleyMod.HelpWanted.Helper.PathStringHelper;
using static weizinai.StardewValleyMod.PiCore.Constant.SItem;
using static weizinai.StardewValleyMod.PiCore.Constant.SNPC;

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
        if (this.quest.target.Value != null || Game1.gameMode == 6)
        {
            Logger<ModEntry>.Trace($"Target for the current resource collection quest has been set to {this.quest.target.Value}.");

            return false;
        }

        this.quest.target.Value = ModEntry.Random.NextBool() ? Clint : Robin;

        return true;
    }

    protected override void SetQuestTitle()
    {
        this.quest.questTitle = Game1.content.LoadString(GetPathString("R", 13640));
    }

    protected override void SetQuestItemId()
    {
        var random = ModEntry.Random;
        var moreQuest = ModConfig.Instance.VanillaConfig.MoreResourceCollectionQuest;
        var possibleItems = new List<string>(4);

        switch (this.quest.target.Value)
        {
            case Clint:
                {
                    possibleItems.AddRange(new[] { CopperOre, IronOre, Coal });

                    if (Utility.GetAllPlayerDeepestMineLevel() > 40)
                    {
                        possibleItems.Add(GoldOre);
                    }

                    if (!moreQuest)
                    {
                        break;
                    }

                    if (Game1.player.mailReceived.Contains("ccVault"))
                    {
                        possibleItems.Add(IridiumOre);
                    }

                    if (Game1.player.mailReceived.Contains("willyHours"))
                    {
                        possibleItems.Add(CinderShard);
                    }

                    break;
                }
            case Robin:
                {
                    possibleItems.AddRange(new[] { Wood, Stone });

                    if (!moreQuest)
                    {
                        break;
                    }

                    if (Game1.player.locationsVisited.Contains("Woods"))
                    {
                        possibleItems.Add(Hardwood);
                    }

                    break;
                }
        }

        this.quest.ItemId.Value = random.ChooseFrom(possibleItems);
        this.quest.number.Value = ItemConfig.TryGetValue(this.quest.ItemId.Value, out var config) ? config.GetRandomNumber() : 1;
        this.item = ItemRegistry.Create(this.quest.ItemId.Value);
    }

    protected override void SetQuestMoneyReward()
    {
        this.quest.reward.Value = this.quest.number.Value * (ItemConfig.TryGetValue(this.quest.ItemId.Value, out var config) ? config.GetReward() : 0);

        var originalReward = this.quest.reward.Value;
        this.quest.reward.Value = (int)(originalReward * ModConfig.Instance.VanillaConfig.ResourceCollectionQuestConfig.RewardMultiplier);
        Logger<ModEntry>.Trace($"The vanilla resource collection quest reward has been adjusted from [{originalReward}] to [{this.quest.reward.Value}].");
    }

    protected override void SetQuestDescription()
    {
        this.quest.parts.Clear();

        if (this.quest.target.Value == Robin)
        {
            this.quest.parts.Add(new DescriptionElement(
                GetPathString("R", 13674),
                this.quest.number.Value,
                this.item
            ));
        }
        else
        {
            this.quest.parts.Add(new DescriptionElement(
                GetPathString("R", 13647),
                this.quest.number.Value,
                this.item,
                new DescriptionElement(GetPathString("R", new[] { 13649, 13650, 13651, 13652 }[this.randomIndex]))
            ));
        }

        this.quest.parts.Add(new DescriptionElement(GetPathString("I", 13607), this.quest.reward.Value));
        this.quest.parts.Add(this.quest.target.Value == Clint ? GetPathString("R", 13688) : "");
    }

    protected override void SetQuestDialogue()
    {
        var random = ModEntry.Random;
        this.quest.dialogueparts.Clear();

        if (this.quest.target.Value == Robin)
        {
            this.quest.dialogueparts.Add(new DescriptionElement(
                GetPathString("R", 13677),
                this.quest.ItemId.Value == Wood
                    ? new DescriptionElement(GetPathString("R", 13678))
                    : new DescriptionElement(GetPathString("R", 13679))
            ));
            this.quest.dialogueparts.Add(GetPathString("R", 13681, 13682, 13683));
        }
        else
        {
            if (this.randomIndex == 3)
            {
                this.quest.dialogueparts.Add(GetPathString("R", 13655));
                this.quest.dialogueparts.Add(GetPathString("R", 13656, 13657, 13658));
                this.quest.dialogueparts.Add(GetPathString("R", 13659));
            }
            else
            {
                this.quest.dialogueparts.Add(GetPathString("R", 13662));
                this.quest.dialogueparts.Add(GetPathString("R", 13656, 13657, 13658));
                this.quest.dialogueparts.Add(random.NextBool()
                    ? new DescriptionElement(GetPathString("R", 13667), new DescriptionElement(GetPathString("R", 13668, 13669, 13670)))
                    : new DescriptionElement(GetPathString("R", 13672)));
                this.quest.dialogueparts.Add(GetPathString("R", 13673));
            }
        }
    }

    protected override void SetQuestObjective()
    {
        this.quest.objective.Value = new DescriptionElement(
            GetPathString("R", 13691),
            "0",
            this.quest.number.Value,
            this.item
        );
    }
}
