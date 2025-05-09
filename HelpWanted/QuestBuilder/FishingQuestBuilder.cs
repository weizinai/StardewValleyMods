using System;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;
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
        if (this.Quest.target.Value != null && this.Quest.ItemId.Value != null)
        {
            Logger.Trace($"Target for the current fishing quest has been set to {this.Quest.target.Value}.");
            Logger.Trace($"ItemId for the current fishing quest has been set to {this.Quest.ItemId.Value}.");
            return false;
        }

        this.Quest.target.Value = this.randomBool ? Demetrius : Willy;
        return true;
    }

    protected override void SetQuestTitle()
    {
        this.Quest.questTitle = Game1.content.LoadString(GetPathString("F", 13227));
    }

    protected override void SetQuestItemId()
    {
        var random = ModEntry.Random;

        if (this.randomBool)
        {
            this.Quest.ItemId.Value = Game1.season switch
            {
                Season.Spring => random.Choose<string>("(O)129", "(O)131", "(O)136", "(O)137", "(O)142", "(O)143", "(O)145", "(O)147"),
                Season.Summer => random.Choose<string>("(O)130", "(O)136", "(O)138", "(O)142", "(O)144", "(O)145", "(O)146", "(O)149", "(O)150"),
                Season.Fall => random.Choose<string>("(O)129", "(O)131", "(O)136", "(O)137", "(O)139", "(O)142", "(O)143", "(O)150"),
                Season.Winter => random.Choose<string>("(O)130", "(O)131", "(O)136", "(O)141", "(O)144", "(O)146", "(O)147", "(O)150", "(O)151"),
                _ => this.Quest.ItemId.Value
            };
        }
        else
        {
            this.Quest.ItemId.Value = Game1.season switch
            {
                Season.Spring => random.Choose<string>("(O)129", "(O)131", "(O)136", "(O)137", "(O)142", "(O)143", "(O)145", "(O)147", "(O)702"),
                Season.Summer => random.Choose<string>("(O)128", "(O)130", "(O)136", "(O)138", "(O)142", "(O)144", "(O)145", "(O)146", "(O)149", "(O)150", "(O)702"),
                Season.Fall => random.Choose<string>("(O)129", "(O)131", "(O)136", "(O)137", "(O)139", "(O)142", "(O)143", "(O)150", "(O)699", "(O)702", "(O)705"),
                Season.Winter => random.Choose<string>("(O)130", "(O)131", "(O)136", "(O)141", "(O)143", "(O)144", "(O)146", "(O)147", "(O)151", "(O)699", "(O)702", "(O)705"),
                _ => this.Quest.ItemId.Value
            };
        }

        this.fish = ItemRegistry.Create(this.Quest.ItemId.Value);
        this.Quest.numberToFish.Value = (int)Math.Ceiling(90.0 / Math.Max(1, this.GetGoldRewardPerItem(this.fish))) + Game1.player.FishingLevel / 5;
    }

    protected override void SetQuestMoneyReward()
    {
        this.Quest.reward.Value = this.Quest.numberToFish.Value * this.GetGoldRewardPerItem(this.fish);

        var originalReward = this.Quest.reward.Value;
        this.Quest.reward.Value = (int)(originalReward * ModConfig.Instance.VanillaConfig.FishingQuestConfig.RewardMultiplier);
        Logger.Trace($"The vanilla fishing quest reward has been adjusted from [{originalReward}] to [{this.Quest.reward.Value}].");
    }

    protected override void SetQuestDescription()
    {
        this.Quest.parts.Clear();

        if (this.randomBool)
        {
            this.Quest.parts.Add(new DescriptionElement(
                GetPathString("F", 13228),
                this.fish,
                this.Quest.numberToFish.Value
            ));
        }
        else
        {
            var isSquid = this.Quest.ItemId.Value == "(O)151";
            this.Quest.parts.Add(isSquid
                ? new DescriptionElement(
                    GetPathString("F", 13248),
                    this.Quest.reward.Value,
                    this.Quest.numberToFish.Value,
                    new DescriptionElement(GetPathString("F", 13253))
                )
                : new DescriptionElement(
                    GetPathString("F", 13248),
                    this.Quest.reward.Value,
                    this.Quest.numberToFish.Value,
                    this.fish
                )
            );
        }

        this.Quest.parts.Add(new DescriptionElement(GetPathString("F", 13274), this.Quest.reward.Value));
        this.Quest.parts.Add(GetPathString("F", 13275));
    }

    protected override void SetQuestDialogue()
    {
        var random = ModEntry.Random;
        this.Quest.dialogueparts.Clear();

        if (this.randomBool)
        {
            this.Quest.dialogueparts.Add(new DescriptionElement(
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
            this.Quest.dialogueparts.Add(new DescriptionElement(GetPathString("F", 13256), this.fish));
            this.Quest.dialogueparts.Add(random.Choose(
                new DescriptionElement(GetPathString("F", 13258)),
                new DescriptionElement(GetPathString("F", 13259)),
                new DescriptionElement(
                    GetPathString("F", 13260),
                    new DescriptionElement(GetPathString("F", 13261, 13262, 13262, 13264, 13265, 13266))
                ),
                new DescriptionElement(GetPathString("F", 13267))
            ));
            this.Quest.dialogueparts.Add(new DescriptionElement(GetPathString("F", 13268)));
        }
    }

    protected override void SetQuestObjective()
    {
        if (this.randomBool)
        {
            var isOctopus = this.Quest.ItemId.Value == "(O)149";
            this.Quest.objective.Value = isOctopus
                ? new DescriptionElement(GetPathString("F", 13243), 0, this.Quest.numberToFish.Value)
                : new DescriptionElement(GetPathString("F", 13244), 0, this.Quest.numberToFish.Value, this.fish);
        }
        else
        {
            var isSquid = this.Quest.ItemId.Value == "(O)151";
            this.Quest.objective.Value = isSquid
                ? new DescriptionElement(GetPathString("F", 13255), 0, this.Quest.numberToFish.Value)
                : new DescriptionElement(GetPathString("F", 13244), 0, this.Quest.numberToFish.Value, this.fish);
        }
    }

    private int GetGoldRewardPerItem(Item item) => item is SObject obj ? obj.Price : (int)(item.salePrice() * 1.5f);
}