using System;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.PiCore.Logging;
using static weizinai.StardewValleyMod.HelpWanted.Helper.PathStringHelper;
using static weizinai.StardewValleyMod.PiCore.Constant.SNPC;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public class FishingQuestBuilder : QuestBuilder<FishingQuest>
{
    private Item fish = null!;
    private readonly bool randomBool;

    public FishingQuestBuilder(FishingQuest quest) : base(quest)
    {
        quest.daysLeft.Value = ModConfig.Instance.VanillaConfig.FishingQuestConfig.Days;

        this.randomBool = ModEntry.Random.NextBool();
    }

    protected override bool TrySetQuestTarget()
    {
        if (this.quest.target.Value != null && this.quest.ItemId.Value != null)
        {
            Logger<ModEntry>.Trace($"Target for the current fishing quest has been set to {this.quest.target.Value}.");
            Logger<ModEntry>.Trace($"ItemId for the current fishing quest has been set to {this.quest.ItemId.Value}.");

            return false;
        }

        this.quest.target.Value = this.randomBool ? Demetrius : Willy;

        return true;
    }

    protected override void SetQuestTitle()
    {
        this.quest.questTitle = Game1.content.LoadString(GetPathString("F", 13227));
    }

    protected override void SetQuestItemId()
    {
        var random = ModEntry.Random;

        if (this.randomBool)
        {
            this.quest.ItemId.Value = Game1.season switch
            {
                Season.Spring => random.Choose<string>("(O)129", "(O)131", "(O)136", "(O)137", "(O)142", "(O)143", "(O)145", "(O)147"),
                Season.Summer => random.Choose<string>("(O)130", "(O)136", "(O)138", "(O)142", "(O)144", "(O)145", "(O)146", "(O)149", "(O)150"),
                Season.Fall => random.Choose<string>("(O)129", "(O)131", "(O)136", "(O)137", "(O)139", "(O)142", "(O)143", "(O)150"),
                Season.Winter => random.Choose<string>("(O)130", "(O)131", "(O)136", "(O)141", "(O)144", "(O)146", "(O)147", "(O)150", "(O)151"),
                _ => this.quest.ItemId.Value
            };
        }
        else
        {
            this.quest.ItemId.Value = Game1.season switch
            {
                Season.Spring => random.Choose<string>("(O)129", "(O)131", "(O)136", "(O)137", "(O)142", "(O)143", "(O)145", "(O)147", "(O)702"),
                Season.Summer => random.Choose<string>("(O)128", "(O)130", "(O)136", "(O)138", "(O)142", "(O)144", "(O)145", "(O)146", "(O)149", "(O)150",
                    "(O)702"),
                Season.Fall => random.Choose<string>("(O)129", "(O)131", "(O)136", "(O)137", "(O)139", "(O)142", "(O)143", "(O)150", "(O)699", "(O)702",
                    "(O)705"),
                Season.Winter => random.Choose<string>("(O)130", "(O)131", "(O)136", "(O)141", "(O)143", "(O)144", "(O)146", "(O)147", "(O)151", "(O)699",
                    "(O)702", "(O)705"),
                _ => this.quest.ItemId.Value
            };
        }

        this.fish = ItemRegistry.Create(this.quest.ItemId.Value);
        this.quest.numberToFish.Value = (int)Math.Ceiling(90.0 / Math.Max(1, this.GetGoldRewardPerItem(this.fish))) + Game1.player.FishingLevel / 5;
    }

    protected override void SetQuestMoneyReward()
    {
        this.quest.reward.Value = this.quest.numberToFish.Value * this.GetGoldRewardPerItem(this.fish);

        var originalReward = this.quest.reward.Value;
        this.quest.reward.Value = (int)(originalReward * ModConfig.Instance.VanillaConfig.FishingQuestConfig.RewardMultiplier);
        Logger<ModEntry>.Trace($"The vanilla fishing quest reward has been adjusted from [{originalReward}] to [{this.quest.reward.Value}].");
    }

    protected override void SetQuestDescription()
    {
        this.quest.parts.Clear();

        if (this.randomBool)
        {
            this.quest.parts.Add(new DescriptionElement(
                GetPathString("F", 13228),
                this.fish,
                this.quest.numberToFish.Value
            ));
        }
        else
        {
            var isSquid = this.quest.ItemId.Value == "(O)151";
            this.quest.parts.Add(isSquid
                ? new DescriptionElement(
                    GetPathString("F", 13248),
                    this.quest.reward.Value,
                    this.quest.numberToFish.Value,
                    new DescriptionElement(GetPathString("F", 13253))
                )
                : new DescriptionElement(
                    GetPathString("F", 13248),
                    this.quest.reward.Value,
                    this.quest.numberToFish.Value,
                    this.fish
                )
            );
        }

        this.quest.parts.Add(new DescriptionElement(GetPathString("F", 13274), this.quest.reward.Value));
        this.quest.parts.Add(GetPathString("F", 13275));
    }

    protected override void SetQuestDialogue()
    {
        var random = ModEntry.Random;
        this.quest.dialogueparts.Clear();

        if (this.randomBool)
        {
            this.quest.dialogueparts.Add(new DescriptionElement(
                GetPathString("F", 13231),
                this.fish,
                random.Choose(
                    new DescriptionElement(GetPathString("F", 13233)),
                    new DescriptionElement(GetPathString("F", 13234)),
                    new DescriptionElement(GetPathString("F", 13235)),
                    new DescriptionElement(GetPathString("F", 13236), this.fish)
                )
            ));
        }
        else
        {
            this.quest.dialogueparts.Add(new DescriptionElement(GetPathString("F", 13256), this.fish));
            this.quest.dialogueparts.Add(random.Choose(
                new DescriptionElement(GetPathString("F", 13258)),
                new DescriptionElement(GetPathString("F", 13259)),
                new DescriptionElement(
                    GetPathString("F", 13260),
                    new DescriptionElement(GetPathString("F", 13261, 13262, 13262, 13264, 13265, 13266))
                ),
                new DescriptionElement(GetPathString("F", 13267))
            ));
            this.quest.dialogueparts.Add(new DescriptionElement(GetPathString("F", 13268)));
        }
    }

    protected override void SetQuestObjective()
    {
        if (this.randomBool)
        {
            var isOctopus = this.quest.ItemId.Value == "(O)149";
            this.quest.objective.Value = isOctopus
                ? new DescriptionElement(GetPathString("F", 13243), 0, this.quest.numberToFish.Value)
                : new DescriptionElement(GetPathString("F", 13244), 0, this.quest.numberToFish.Value, this.fish);
        }
        else
        {
            var isSquid = this.quest.ItemId.Value == "(O)151";
            this.quest.objective.Value = isSquid
                ? new DescriptionElement(GetPathString("F", 13255), 0, this.quest.numberToFish.Value)
                : new DescriptionElement(GetPathString("F", 13244), 0, this.quest.numberToFish.Value, this.fish);
        }
    }

    private int GetGoldRewardPerItem(Item item)
    {
        return item is SObject obj ? obj.Price : (int)(item.salePrice() * 1.5f);
    }
}
