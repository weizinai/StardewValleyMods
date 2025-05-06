using StardewValley;
using StardewValley.Extensions;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Manager;
using static weizinai.StardewValleyMod.HelpWanted.Helper.PathStringHelper;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public class ItemDeliveryQuestBuilder : QuestBuilder<ItemDeliveryQuest>
{
    private NPC? targetNPC;
    private Item item = null!;
    private readonly double randomDouble;
    private readonly bool randomBool_Maru;
    private readonly bool randomBool_Abigail;

    public ItemDeliveryQuestBuilder(ItemDeliveryQuest quest) : base(quest)
    {
        quest.daysLeft.Value = ModConfig.Instance.VanillaConfig.ItemDeliveryQuestConfig.Days;

        var random = ModEntry.Random;
        this.randomDouble = random.NextDouble();
        this.randomBool_Maru = random.NextBool();
        this.randomBool_Abigail = random.NextBool();
    }

    protected override bool TrySetQuestTarget()
    {
        if (this.Quest.target.Value != null)
        {
            Logger.Trace($"Target for the current item delivery quest has been set to {this.Quest.target.Value}.");
            return false;
        }

        var validTargets = this.Quest.GetValidTargetList();

        if (Game1.player.friendshipData is not { Length: > 0 } || validTargets.Count <= 0)
        {
            Logger.Error("No NPC is assigned as the item delivery target in step 1.");
            return false;
        }

        this.targetNPC = ModEntry.Random.ChooseFrom(validTargets);

        if (this.targetNPC == null)
        {
            Logger.Error("No NPC is assigned as the item delivery target in step 2.");
            return false;
        }

        this.Quest.target.Value = this.targetNPC.Name;

        if (this.Quest.target.Value == "Wizard" && !Game1.player.mailReceived.Contains("wizardJunimoNote") && !Game1.player.mailReceived.Contains("JojaMember"))
        {
            this.Quest.target.Value = "Demetrius";
            this.targetNPC = Game1.getCharacterFromName(this.Quest.target.Value);
        }

        return true;
    }

    protected override void SetQuestTitle()
    {
        this.Quest.questTitle = Game1.content.LoadString(
            "Strings\\1_6_Strings:ItemDeliveryQuestTitle",
            NPC.GetDisplayName(this.Quest.target.Value)
        );
    }

    protected override void SetQuestItemId()
    {
        if (Game1.season != Season.Winter && this.randomDouble < 0.15)
        {
            // this.quest.ItemId.Value = this.Random.ChooseFrom(Utility.possibleCropsAtThisTime(Game1.season, Game1.dayOfMonth <= 7));
            this.Quest.ItemId.Value = QuestItemManager.Instance.GetRandomCrop(this.Quest.target.Value);
            this.Quest.ItemId.Value = ItemRegistry.QualifyItemId(this.Quest.ItemId.Value) ?? this.Quest.ItemId.Value;
        }
        else
        {
            // var rawItemId = Utility.getRandomItemFromSeason(Game1.season, 1000, true);
            var rawItemId = QuestItemManager.Instance.GetRandomItem(this.Quest.target.Value);
            this.Quest.ItemId.Value = rawItemId switch
            {
                "-5" => "(O)176",
                "-6" => "(O)184",
                _ => ItemRegistry.QualifyItemId(rawItemId) ?? rawItemId
            };
        }
        this.item = ItemRegistry.Create(this.Quest.ItemId.Value);
    }

    protected override void SetQuestMoneyReward()
    {
        if (this.Quest.dailyQuest.Value || this.Quest.moneyReward.Value == 0)
        {
            var reward = this.Quest.GetGoldRewardPerItem(this.item);
            this.Quest.moneyReward.Value = (int)(reward * ModConfig.Instance.VanillaConfig.ItemDeliveryQuestConfig.RewardMultiplier);
            Logger.Trace($"The vanilla item delivery quest reward has been adjusted from [{reward}] to [{this.Quest.moneyReward.Value}].");
        }
    }

    protected override void SetQuestDescription()
    {
        var random = ModEntry.Random;
        this.Quest.parts.Clear();

        switch (this.Quest.target.Value)
        {
            case "Wizard":
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13546, 13548, 13551, 13553), this.item));
                break;
            }
            case "Haley":
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13557, 13560), this.item));
                break;
            }
            case "Sam":
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13568, 13571), this.item));
                break;
            }
            case "Maru":
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("I", this.randomBool_Maru ? 13580 : 13583), this.item));
                break;
            }
            case "Abigail":
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("I", this.randomBool_Abigail ? 13590 : 13593), this.item));
                break;
            }
            default:
            {
                if (Game1.season != Season.Winter && this.randomDouble < 0.15)
                {
                    switch (this.Quest.target.Value)
                    {
                        case "Demetrius":
                        {
                            this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13311, 13314), this.item));
                            break;
                        }
                        case "Marnie":
                        {
                            this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13317, 13320), this.item));
                            break;
                        }
                        case "Sebastian":
                        {
                            this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13324, 13327), this.item));
                            break;
                        }
                        default:
                        {
                            this.Quest.parts.Add(GetPathString("I", 13299, 13300, 13301));
                            this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13302, 13303, 13304), this.item));
                            this.Quest.parts.Add(random.Choose(
                                GetPathString("I", 13306),
                                GetPathString("I", 13307),
                                "",
                                GetPathString("I", 13308)
                            ));
                            this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13620), this.targetNPC));
                            break;
                        }
                    }
                }
                else
                {
                    switch (this.item)
                    {
                        case SObject { Type: "Cooking" } when this.Quest.target.Value != "Wizard":
                        {
                            this.HandlerCookingQuest();
                            break;
                        }
                        case SObject { Edibility: > 0 } when random.NextBool():
                        {
                            this.HandlerEdibleQuest();
                            break;
                        }
                        case SObject { Edibility: < 0 } when random.NextBool():
                        case not SObject when random.NextBool():
                        {
                            if (this.Quest.target.Value.Equals("Emily"))
                            {
                                this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13473, 13476), this.item));
                            }
                            else
                            {
                                this.Quest.parts.Add(new DescriptionElement(
                                    GetPathString("I", 13464),
                                    this.item,
                                    new DescriptionElement(GetPathString("I", 13465, 13466, 13467, 13468, 13469))
                                ));
                                this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13620), this.targetNPC));
                            }
                            break;
                        }
                        default:
                        {
                            this.HandlerDefaultQuest();
                            break;
                        }
                    }
                }
                break;
            }
        }

        this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13607), this.Quest.moneyReward.Value));
        this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13608, 13610, 13612), this.targetNPC));
    }

    protected override void SetQuestDialogue()
    {
        var random = ModEntry.Random;
        this.Quest.dialogueparts.Clear();

        switch (this.Quest.target.Value)
        {
            case "Wizard":
            {
                this.Quest.dialogueparts.Add(GetPathString("I", 13555));
                break;
            }
            case "Haley":
            {
                this.Quest.dialogueparts.Add(GetPathString("I", 13566));
                break;
            }
            case "Sam":
            {
                this.Quest.dialogueparts.Add(new DescriptionElement(GetPathString("I", 13577)));
                break;
            }
            case "Maru":
            {
                this.Quest.dialogueparts.Add(new DescriptionElement(GetPathString("I", this.randomBool_Maru ? 13585 : 13587)));
                break;
            }
            case "Abigail":
            {
                this.Quest.dialogueparts.Add(new DescriptionElement(GetPathString("I", this.randomBool_Abigail ? 13597 : 13599)));
                break;
            }
            case "Sebastian":
            {
                this.Quest.dialogueparts.Add(GetPathString("I", 13602));
                break;
            }
            case "Elliott":
            {
                this.Quest.dialogueparts.Add(new DescriptionElement(GetPathString("I", 13604), this.item));
                break;
            }
            default:
            {
                this.Quest.dialogueparts.Add(random.NextBool(0.3) || this.Quest.target.Value == "Evelyn"
                    ? new DescriptionElement(GetPathString("I", 13526))
                    : new DescriptionElement(GetPathString("I", 13527, 13528)));
                this.Quest.dialogueparts.Add(random.NextBool(0.3)
                    ? new DescriptionElement(GetPathString("I", 13530), this.item)
                    : random.NextBool()
                        ? new DescriptionElement(GetPathString("I", 13532))
                        : new DescriptionElement(GetPathString("I", 13534, 13535, 13536)));
                this.Quest.dialogueparts.Add(GetPathString("I", 13538, 13539, 13540));
                this.Quest.dialogueparts.Add(GetPathString("I", 13542, 13543, 13544));
                break;
            }
        }
    }

    protected override void SetQuestObjective()
    {
        this.Quest.objective.Value = new DescriptionElement(
            GetPathString("I", 13614),
            this.targetNPC,
            this.item
        );
    }

    private void HandlerCookingQuest()
    {
        var random = ModEntry.Random;

        if (random.NextDouble() < 0.33)
        {
            var questStrings = new[]
            {
                new(GetPathString("I", 13336)),
                new(GetPathString("I", 13337)),
                new(GetPathString("I", 13338)),
                new(GetPathString("I", 13339)),
                new(GetPathString("I", 13340)),
                new(GetPathString("I", 13341)),
                Game1.samBandName != Game1.content.LoadString(GetPathString("G", 2156))
                    ? new DescriptionElement(GetPathString("I", 13347), new DescriptionElement(GetPathString("G", 2156)))
                    : Game1.elliottBookName != Game1.content.LoadString(GetPathString("G", 2157))
                        ? new DescriptionElement(GetPathString("I", 13342), new DescriptionElement(GetPathString("G", 2157)))
                        : new DescriptionElement(GetPathString("I", 13346)),
                new(GetPathString("I", 13349)),
                new(GetPathString("I", 13350)),
                new(GetPathString("I", 13351)),
                Game1.season switch
                {
                    Season.Winter => new DescriptionElement(GetPathString("I", 13353)),
                    Season.Summer => new DescriptionElement(GetPathString("I", 13355)),
                    _ => new DescriptionElement(GetPathString("I", 13356))
                },
                new(GetPathString("I", 13357))
            };
            this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13333, 13334), this.item, random.ChooseFrom(questStrings)));
            this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13620), this.targetNPC));
        }
        else
        {
            if (this.Quest.target.Value.Equals("Sebastian"))
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13378, 13381), this.item));
            }
            else
            {
                var day = (Game1.dayOfMonth % 7) switch
                {
                    0 => new DescriptionElement(GetPathString("G", 3042)),
                    1 => new DescriptionElement(GetPathString("G", 3043)),
                    2 => new DescriptionElement(GetPathString("G", 3044)),
                    3 => new DescriptionElement(GetPathString("G", 3045)),
                    4 => new DescriptionElement(GetPathString("G", 3046)),
                    5 => new DescriptionElement(GetPathString("G", 3047)),
                    _ => new DescriptionElement(GetPathString("G", 3048)),
                };
                var questDescriptions1 = new DescriptionElement[]
                {
                    new(GetPathString("I", 13360), this.item),
                    new(GetPathString("I", 13364), this.item),
                    new(GetPathString("I", 13367), this.item),
                    new(GetPathString("I", 13370), this.item),
                    new(GetPathString("I", 13373), day, this.item, this.targetNPC)
                };
                var questDescriptions2 = new DescriptionElement[]
                {
                    new(GetPathString("I", 13620), this.targetNPC),
                    new(GetPathString("I", 13620), this.targetNPC),
                    new(GetPathString("I", 13620), this.targetNPC),
                    new(GetPathString("I", 13620), this.targetNPC),
                    new("")
                };
                var questDescriptions3 = new DescriptionElement[]
                {
                    new(""),
                    new(""),
                    new(""),
                    new(""),
                    new("")
                };
                var randomInt = random.Next(questDescriptions1.Length);
                this.Quest.parts.Add(questDescriptions1[randomInt]);
                this.Quest.parts.Add(questDescriptions2[randomInt]);
                this.Quest.parts.Add(questDescriptions3[randomInt]);
            }
        }
    }

    private void HandlerEdibleQuest()
    {
        var random = ModEntry.Random;

        switch (this.Quest.target.Value)
        {
            case "Demetrius":
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13311, 13314), this.item));
                break;
            }
            case "Marnie":
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13317, 13320), this.item));
                break;
            }
            case "Harvey":
            {
                this.Quest.parts.Add(new DescriptionElement(
                    GetPathString("I", 13446),
                    this.item,
                    new DescriptionElement(GetPathString("I", 13448, 13449, 13450, 13451, 13452, 13453, 13454, 13455, 13456, 13457, 13458, 13459))
                ));
                break;
            }
            case "Gus" when random.NextDouble() < 0.6:
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13462), this.item));
                break;
            }
            default:
            {
                if (random.NextDouble() < 0.33)
                {
                    var questStrings = new[]
                    {
                        new(GetPathString("I", 13336)),
                        new(GetPathString("I", 13337)),
                        new(GetPathString("I", 13338)),
                        new(GetPathString("I", 13339)),
                        new(GetPathString("I", 13340)),
                        new(GetPathString("I", 13341)),
                        Game1.samBandName != Game1.content.LoadString(GetPathString("G", 2156))
                            ? new DescriptionElement(GetPathString("I", 13347), new DescriptionElement(GetPathString("G", 2156)))
                            : Game1.elliottBookName != Game1.content.LoadString(GetPathString("G", 2157))
                                ? new DescriptionElement(GetPathString("I", 13342), new DescriptionElement(GetPathString("G", 2157)))
                                : new DescriptionElement(GetPathString("I", 13346)),
                        new(GetPathString("I", 13420)),
                        new(GetPathString("I", 13421)),
                        new(GetPathString("I", 13422)),
                        Game1.season switch
                        {
                            Season.Winter => new DescriptionElement(GetPathString("I", 13424)),
                            Season.Summer => new DescriptionElement(GetPathString("I", 13426)),
                            _ => new DescriptionElement(GetPathString("I", 13427))
                        },
                        new(GetPathString("I", 13357))
                    };
                    this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13333, 13334), this.item, random.ChooseFrom(questStrings)));
                    this.Quest.parts.Add(new DescriptionElement(GetPathString("I", 13620), this.targetNPC));
                }
                else
                {
                    var questDescriptions1 = new DescriptionElement[]
                    {
                        new(
                            GetPathString("I", 13383),
                            this.item,
                            new DescriptionElement(GetPathString("I", 13385, 13386, 13387, 13388, 13389, 13390, 13391, 13392, 13393, 13394, 13395, 13396)),
                            new DescriptionElement(GetPathString("I", 13400), this.item)
                        )
                    };
                    var questDescriptions2 = new DescriptionElement[]
                    {
                        new(random.Choose("", GetPathString("I", 13398))),
                        new(random.Choose("", GetPathString("I", 13402)))
                    };
                    var questDescriptions3 = new DescriptionElement[]
                    {
                        new(GetPathString("I", 13620), this.targetNPC),
                        new(GetPathString("I", 13620), this.targetNPC)
                    };
                    var index = random.Next(questDescriptions1.Length);
                    this.Quest.parts.Add(questDescriptions1[index]);
                    this.Quest.parts.Add(questDescriptions2[index]);
                    this.Quest.parts.Add(questDescriptions3[index]);
                }
                break;
            }
        }
    }

    private void HandlerDefaultQuest()
    {
        var random = ModEntry.Random;

        var questDescriptions1 = new DescriptionElement[]
        {
            new(GetPathString("I", 13480), this.targetNPC, this.item),
            new(GetPathString("I", 13481), this.item),
            new(GetPathString("I", 13485), this.item),
            new(GetPathString("I", 13491, 13492), this.item),
            new(GetPathString("I", 13494), this.item),
            new(GetPathString("I", 13497), this.item),
            new(
                GetPathString("I", 13500),
                this.item,
                new DescriptionElement(GetPathString("I", 13502, 13503, 13504, 13505, 13506, 13507, 13508, 13509, 13510, 13511, 13512, 13513))
            ),
            new(GetPathString("I", 13518), this.targetNPC, this.item),
            new(GetPathString("I", 13520, 13523), this.item)
        };
        var questDescriptions2 = new DescriptionElement[]
        {
            new(""),
            new(random.Choose(
                GetPathString("I", 13482),
                "",
                GetPathString("I", 13483)
            )),
            new(random.Choose(
                GetPathString("I", 13487),
                GetPathString("I", 13488),
                "",
                GetPathString("I", 13489)
            )),
            new(GetPathString("I", 13620), this.targetNPC),
            new(GetPathString("I", 13620), this.targetNPC),
            new(GetPathString("I", 13620), this.targetNPC),
            new(GetPathString("I", 13514, 13516)),
            new(""),
            new(GetPathString("I", 13620), this.targetNPC)
        };
        var questDescriptions3 = new DescriptionElement[]
        {
            new(""),
            new(GetPathString("I", 13620), this.targetNPC),
            new(GetPathString("I", 13620), this.targetNPC),
            new(""),
            new(""),
            new(""),
            new(GetPathString("I", 13620), this.targetNPC),
            new(""),
            new("")
        };
        var index = random.Next(questDescriptions1.Length);
        this.Quest.parts.Add(questDescriptions1[index]);
        this.Quest.parts.Add(questDescriptions2[index]);
        this.Quest.parts.Add(questDescriptions3[index]);
    }
}