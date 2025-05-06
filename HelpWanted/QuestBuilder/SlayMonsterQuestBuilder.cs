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
using weizinai.StardewValleyMod.HelpWanted.Repository;
using static weizinai.StardewValleyMod.HelpWanted.Helper.PathStringHelper;

namespace weizinai.StardewValleyMod.HelpWanted.QuestBuilder;

public class SlayMonsterQuestBuilder : QuestBuilder<SlayMonsterQuest>
{
    private static readonly Dictionary<string, MonsterConfig> ItemConfig = new()
    {
        // 原版任务矿井怪物
        [MonsterRepository.GreenSlime] = new MonsterConfig("Lewis", 60, 4, 11, 2),
        [MonsterRepository.RockCrab] = new MonsterConfig("Demetrius", 75, 2, 6),
        [MonsterRepository.Duggy] = new MonsterConfig("Clint", 150, 2, 4),
        [MonsterRepository.FrostJelly] = new MonsterConfig("Lewis", 85, 4, 11, 2),
        [MonsterRepository.DustSpirit] = new MonsterConfig("Wizard", 60, 10, 21),
        [MonsterRepository.Ghost] = new MonsterConfig("Wizard", 250, 2, 4),
        [MonsterRepository.Skeleton] = new MonsterConfig("Wizard", 100, 6, 12),
        [MonsterRepository.Sludge] = new MonsterConfig("Lewis", 125, 4, 11, 2),
        [MonsterRepository.LavaCrab] = new MonsterConfig("Demetrius", 180, 2, 6),
        [MonsterRepository.SquidKid] = new MonsterConfig("Wizard", 350, 1, 3),
        // 模组任务矿井怪物
        [MonsterRepository.Bug] = new MonsterConfig("Marlon", 30),
        [MonsterRepository.Grub] = new MonsterConfig("Marlon", 50, 10, 21),
        [MonsterRepository.Fly] = new MonsterConfig("Marlon", 60),
        [MonsterRepository.Bat] = new MonsterConfig("Marlon", 80),
        [MonsterRepository.StoneGolem] = new MonsterConfig("Wizard", 80),
        [MonsterRepository.FrostBat] = new MonsterConfig("Marlon", 100),
        [MonsterRepository.LavaBat] = new MonsterConfig("Marlon", 150),
        [MonsterRepository.ShadowBrute] = new MonsterConfig("Wizard", 150),
        [MonsterRepository.ShadowShaman] = new MonsterConfig("Wizard", 150),
        [MonsterRepository.MetalHead] = new MonsterConfig("Wizard", 150),
        [MonsterRepository.BigSlime] = new MonsterConfig("Sandy", 180),
        [MonsterRepository.Mummy] = new MonsterConfig("Wizard", 400),
        [MonsterRepository.Serpent] = new MonsterConfig("Sandy", 300),
        [MonsterRepository.CarbonGhost] = new MonsterConfig("Wizard", 300),
        [MonsterRepository.PepperRex] = new MonsterConfig("Sandy", 600),
        [MonsterRepository.IridiumCrab] = new MonsterConfig("Demetrius", 400),
        [MonsterRepository.IridiumBat] = new MonsterConfig("Sandy", 400),
        // 模组任务火山怪物
        [MonsterRepository.DwarvishSentry] = new MonsterConfig("Leo", 300),
        [MonsterRepository.FalseMagmaCap] = new MonsterConfig("Leo", 300),
        [MonsterRepository.HotHead] = new MonsterConfig("Leo", 300),
        [MonsterRepository.LavaLurk] = new MonsterConfig("Leo", 300),
        [MonsterRepository.MagmaDuggy] = new MonsterConfig("Clint", 500, 1, 3),
        [MonsterRepository.MagmaSparker] = new MonsterConfig("Leo", 300),
        [MonsterRepository.MagmaSprite] = new MonsterConfig("Leo", 300),
        [MonsterRepository.TigerSlime] = new MonsterConfig("Leo", 250)
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
            case MonsterRepository.GreenSlime:
            {
                this.Quest.parts.Add(new DescriptionElement(
                    GetPathString("S", 13723),
                    this.Quest.numberToKill.Value,
                    new DescriptionElement(GetPathString("S", 13728))
                ));
                break;
            }
            case MonsterRepository.Duggy:
            case MonsterRepository.MagmaDuggy:
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("S", 13711), this.Quest.numberToKill.Value));
                break;
            }
            case MonsterRepository.FrostJelly:
            {
                this.Quest.parts.Add(new DescriptionElement(
                    GetPathString("S", 13723),
                    this.Quest.numberToKill.Value,
                    new DescriptionElement(GetPathString("S", 13725))
                ));
                break;
            }
            case MonsterRepository.Sludge:
            {
                this.Quest.parts.Add(new DescriptionElement(
                    GetPathString("S", 13723),
                    this.Quest.numberToKill.Value,
                    new DescriptionElement(GetPathString("S", 13727))
                ));
                break;
            }
            case MonsterRepository.RockCrab:
            case MonsterRepository.LavaCrab:
            {
                this.Quest.parts.Add(new DescriptionElement(GetPathString("S", 13747), this.Quest.numberToKill.Value));
                break;
            }
            case MonsterRepository.DustSpirit:
            case MonsterRepository.Ghost:
            case MonsterRepository.Skeleton:
            case MonsterRepository.SquidKid:
            case MonsterRepository.StoneGolem:
            case MonsterRepository.ShadowBrute:
            case MonsterRepository.ShadowShaman:
            case MonsterRepository.MetalHead:
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
            case MonsterRepository.Mummy:
            case MonsterRepository.CarbonGhost:
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
            case MonsterRepository.GreenSlime:
            case MonsterRepository.FrostJelly:
            case MonsterRepository.Sludge:
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
            case MonsterRepository.RockCrab:
            case MonsterRepository.LavaCrab:
            {
                this.Quest.dialogueparts.Add(new DescriptionElement(GetPathString("S", 13750), this.Quest.monster.Value));
                break;
            }
            case MonsterRepository.Duggy:
            case MonsterRepository.MagmaDuggy:
            {
                this.Quest.dialogueparts.Add(GetPathString("S", 13760));
                break;
            }
            case MonsterRepository.DustSpirit:
            case MonsterRepository.Ghost:
            case MonsterRepository.Skeleton:
            case MonsterRepository.SquidKid:
            case MonsterRepository.StoneGolem:
            case MonsterRepository.ShadowBrute:
            case MonsterRepository.ShadowShaman:
            case MonsterRepository.MetalHead:
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
            case MonsterRepository.Mummy:
            case MonsterRepository.CarbonGhost:
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