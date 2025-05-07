using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Manager;
using weizinai.StardewValleyMod.HelpWanted.Model;
using weizinai.StardewValleyMod.PiCore.Constant;
using static weizinai.StardewValleyMod.HelpWanted.Helper.PathStringHelper;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public class SlayMonsterQuestBuilder : QuestBuilder<SlayMonsterQuest>
{
    private static readonly Dictionary<string, MonsterConfig> ItemConfig = new()
    {
        // 原版任务矿井怪物
        [SMonster.GreenSlime] = new MonsterConfig("Lewis", 60, 4, 11, 2),
        [SMonster.RockCrab] = new MonsterConfig("Demetrius", 75, 2, 6),
        [SMonster.Duggy] = new MonsterConfig("Clint", 150, 2, 4),
        [SMonster.FrostJelly] = new MonsterConfig("Lewis", 85, 4, 11, 2),
        [SMonster.DustSpirit] = new MonsterConfig("Wizard", 60, 10, 21),
        [SMonster.Ghost] = new MonsterConfig("Wizard", 250, 2, 4),
        [SMonster.Skeleton] = new MonsterConfig("Wizard", 100, 6, 12),
        [SMonster.Sludge] = new MonsterConfig("Lewis", 125, 4, 11, 2),
        [SMonster.LavaCrab] = new MonsterConfig("Demetrius", 180, 2, 6),
        [SMonster.SquidKid] = new MonsterConfig("Wizard", 350, 1, 3),
        // 模组任务矿井怪物
        [SMonster.Bug] = new MonsterConfig("Marlon", 30),
        [SMonster.Grub] = new MonsterConfig("Marlon", 50, 10, 21),
        [SMonster.Fly] = new MonsterConfig("Marlon", 60),
        [SMonster.Bat] = new MonsterConfig("Marlon", 80),
        [SMonster.StoneGolem] = new MonsterConfig("Wizard", 80),
        [SMonster.FrostBat] = new MonsterConfig("Marlon", 100),
        [SMonster.LavaBat] = new MonsterConfig("Marlon", 150),
        [SMonster.ShadowBrute] = new MonsterConfig("Wizard", 150),
        [SMonster.ShadowShaman] = new MonsterConfig("Wizard", 150),
        [SMonster.MetalHead] = new MonsterConfig("Wizard", 150),
        [SMonster.BigSlime] = new MonsterConfig("Sandy", 180),
        [SMonster.Mummy] = new MonsterConfig("Wizard", 400),
        [SMonster.Serpent] = new MonsterConfig("Sandy", 300),
        [SMonster.CarbonGhost] = new MonsterConfig("Wizard", 300),
        [SMonster.PepperRex] = new MonsterConfig("Sandy", 600),
        [SMonster.IridiumCrab] = new MonsterConfig("Demetrius", 400),
        [SMonster.IridiumBat] = new MonsterConfig("Sandy", 400),
        // 模组任务火山怪物
        [SMonster.DwarvishSentry] = new MonsterConfig("Leo", 300),
        [SMonster.FalseMagmaCap] = new MonsterConfig("Leo", 300),
        [SMonster.HotHead] = new MonsterConfig("Leo", 300),
        [SMonster.LavaLurk] = new MonsterConfig("Leo", 300),
        [SMonster.MagmaDuggy] = new MonsterConfig("Clint", 500, 1, 3),
        [SMonster.MagmaSparker] = new MonsterConfig("Leo", 300),
        [SMonster.MagmaSprite] = new MonsterConfig("Leo", 300),
        [SMonster.TigerSlime] = new MonsterConfig("Leo", 250)
    };

    public SlayMonsterQuestBuilder(SlayMonsterQuest quest) : base(quest)
    {
        quest.daysLeft.Value = ModConfig.Instance.VanillaConfig.SlayMonsterQuestConfig.Days;
    }

    protected override bool TrySetQuestTarget()
    {
        this.Quest.target.Value = ItemConfig.TryGetValue(this.Quest.monsterName.Value, out var config) ? config.GetTarget() : "Marlon";

        if (this.Quest.target.Value == "Wizard" && !this.IsWizardAvailable()) this.Quest.target.Value = "Lewis";

        return true;
    }

    protected override void SetQuestTitle()
    {
        this.Quest.questTitle = Game1.content.LoadString(GetPathString("S", 13696));
    }

    protected override void SetQuestItemId()
    {
        this.Quest.monsterName.Value = QuestMonsterManager.Instance.GetRandomMonster();
        this.Quest.monster.Value = new Monster(this.Quest.monsterName.Value, Vector2.Zero);
        this.Quest.numberToKill.Value = ItemConfig.TryGetValue(this.Quest.monsterName.Value, out var config) ? config.GetRandomNumber() : 1;
    }

    protected override void SetQuestMoneyReward()
    {
        var reward = ItemConfig.TryGetValue(this.Quest.monsterName.Value, out var config) ? config.GetReward() : 0;
        this.Quest.reward.Value = this.Quest.numberToKill.Value * reward;

        var originalReward = this.Quest.reward.Value;
        this.Quest.reward.Value = (int)(originalReward * ModConfig.Instance.VanillaConfig.SlayMonsterQuestConfig.RewardMultiplier);
        Logger.Trace($"The vanilla slay monster quest reward has been adjusted from [{originalReward}] to [{this.Quest.reward.Value}].");
    }

    protected override void SetQuestDescription()
    {
        this.Quest.parts.Clear();

        switch (this.Quest.monsterName.Value)
        {
            case SMonster.GreenSlime:
            {
                this.Quest.parts.Add(new DescriptionElement(
                    GetPathString("S", 13723),
                    this.Quest.numberToKill.Value,
                    new DescriptionElement(GetPathString("S", 13728))
                ));
                break;
            }
            case SMonster.Duggy:
            case SMonster.MagmaDuggy:
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("S", 13711), this.Quest.numberToKill.Value));
                break;
            }
            case SMonster.FrostJelly:
            {
                this.Quest.parts.Add(new DescriptionElement(
                    GetPathString("S", 13723),
                    this.Quest.numberToKill.Value,
                    new DescriptionElement(GetPathString("S", 13725))
                ));
                break;
            }
            case SMonster.Sludge:
            {
                this.Quest.parts.Add(new DescriptionElement(
                    GetPathString("S", 13723),
                    this.Quest.numberToKill.Value,
                    new DescriptionElement(GetPathString("S", 13727))
                ));
                break;
            }
            case SMonster.RockCrab:
            case SMonster.LavaCrab:
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("S", 13747), this.Quest.numberToKill.Value));
                break;
            }
            case SMonster.DustSpirit:
            case SMonster.Ghost:
            case SMonster.Skeleton:
            case SMonster.SquidKid:
            case SMonster.StoneGolem:
            case SMonster.ShadowBrute:
            case SMonster.ShadowShaman:
            case SMonster.MetalHead:
            {
                if (this.IsWizardAvailable())
                {
                    this.Quest.parts.Add(new DescriptionElement(
                        GetPathString("S", 13752),
                        this.Quest.monster.Value,
                        this.Quest.numberToKill.Value,
                        new DescriptionElement(GetPathString("S", 13755, 13756, 13757))
                    ));
                }
                else
                {
                    this.AddDefaultDescription();
                }
                break;
            }
            case SMonster.Mummy:
            case SMonster.CarbonGhost:
            {
                this.Quest.parts.Add(new DescriptionElement(
                    GetPathString("S", 13752),
                    this.Quest.monster.Value,
                    this.Quest.numberToKill.Value,
                    new DescriptionElement(GetPathString("S", 13755, 13756, 13757))
                ));
                break;
            }
            default:
            {
                this.AddDefaultDescription();
                break;
            }
        }

        this.Quest.parts.Add(new DescriptionElement(GetPathString("F", 13274), this.Quest.reward.Value));
    }

    protected override void SetQuestDialogue()
    {
        var random = ModEntry.Random;
        this.Quest.dialogueparts.Clear();

        switch (this.Quest.monsterName.Value)
        {
            case SMonster.GreenSlime:
            case SMonster.FrostJelly:
            case SMonster.Sludge:
            {
                this.Quest.dialogueparts.Add(GetPathString("S", 13730));
                if (random.NextBool())
                {
                    this.Quest.dialogueparts.Add(GetPathString("S", 13731));
                    this.Quest.dialogueparts.Add(GetPathString("S", 13731, 13733));
                    this.Quest.dialogueparts.Add(new DescriptionElement(
                        GetPathString("S", 13734),
                        new DescriptionElement(GetPathString("S", 13735, 13736)),
                        new DescriptionElement(GetPathString("D", 798, 799, 800, 801, 802, 803, 804, 805, 806, 807, 808, 809, 810)),
                        new DescriptionElement(GetPathString("S", 13740, 13741, 13732))
                    ));
                }
                else
                {
                    this.Quest.dialogueparts.Add(GetPathString("S", 13744));
                }
                break;
            }
            case SMonster.RockCrab:
            case SMonster.LavaCrab:
            {
                this.Quest.dialogueparts.Add(new DescriptionElement(GetPathString("S", 13750), this.Quest.monster.Value));
                break;
            }
            case SMonster.Duggy:
            case SMonster.MagmaDuggy:
            {
                this.Quest.dialogueparts.Add(GetPathString("S", 13760));
                break;
            }
            case SMonster.DustSpirit:
            case SMonster.Ghost:
            case SMonster.Skeleton:
            case SMonster.SquidKid:
            case SMonster.StoneGolem:
            case SMonster.ShadowBrute:
            case SMonster.ShadowShaman:
            case SMonster.MetalHead:
            {
                if (this.IsWizardAvailable())
                {
                    this.Quest.dialogueparts.Add(GetPathString("S", 13760));
                }
                else
                {
                    this.AddDefaultDialogue();
                }
                break;
            }
            case SMonster.Mummy:
            case SMonster.CarbonGhost:
            {
                this.Quest.dialogueparts.Add(GetPathString("S", 13760));
                break;
            }
            default:
            {
                this.AddDefaultDialogue();
                break;
            }
        }
    }

    protected override void SetQuestObjective()
    {
        this.Quest.objective.Value = new DescriptionElement(
            GetPathString("S", 13770),
            "0",
            this.Quest.numberToKill.Value,
            this.Quest.monster.Value
        );
    }

    public override void BuildQuest()
    {
        if (this.Quest.target.Value != null && this.Quest.monsterName.Value != null)
        {
            Logger.Trace($"Target for the current slay monster quest has been set to {this.Quest.target.Value}.");
            Logger.Trace($"Monster for the current slay monster quest has been set to {this.Quest.monsterName.Value}.");
            return;
        }

        this.SetQuestItemId();
        this.TrySetQuestTarget();
        this.SetQuestTitle();
        this.SetQuestMoneyReward();
        this.SetQuestDescription();
        this.SetQuestDialogue();
        this.SetQuestObjective();
    }

    private void AddDefaultDescription()
    {
        this.Quest.parts.Add(new DescriptionElement(
            GetPathString("S", 13764),
            this.Quest.numberToKill.Value,
            this.Quest.monster.Value
        ));
    }

    private void AddDefaultDialogue()
    {
        this.Quest.dialogueparts.Add(GetPathString("S", 13767));
    }

    private bool IsWizardAvailable()
    {
        return Utility.doesAnyFarmerHaveMail("wizardJunimoNote") || Utility.doesAnyFarmerHaveMail("JojaMember");
    }
}