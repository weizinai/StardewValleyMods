using System.Collections.Generic;
using StardewValley;
using StardewValley.Extensions;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.PiCore.Constant;

namespace weizinai.StardewValleyMod.HelpWanted.Manager;

public class QuestMonsterManager
{
    public static QuestMonsterManager Instance { get; } = new();

    private readonly List<string> possibleMonsters = new();

    public string GetRandomMonster()
    {
        this.possibleMonsters.AddRange(this.GetVanillaMineShaftMonsters());
        if (ModConfig.Instance.VanillaConfig.MoreSlayMonsterQuest)
        {
            this.possibleMonsters.AddRange(this.GetModMineShaftMonsters());
            this.possibleMonsters.AddRange(this.GetModVolcanoDungeonMonsters());
        }

        return ModEntry.Random.ChooseFrom(this.possibleMonsters);
    }

    public void ClearCache()
    {
        this.possibleMonsters.Clear();
    }

    private IEnumerable<string> GetVanillaMineShaftMonsters()
    {
        var mineLevel = Utility.GetAllPlayerDeepestMineLevel();

        switch (mineLevel)
        {
            case <= 40:
            {
                yield return SMonster.GreenSlime;
                if (mineLevel > 10) yield return SMonster.RockCrab;
                if (mineLevel > 30) yield return SMonster.Duggy;
                break;
            }
            case <= 80:
            {
                yield return SMonster.FrostJelly;
                yield return SMonster.DustSpirit;
                if (mineLevel > 50) yield return SMonster.Ghost;
                if (mineLevel > 70) yield return SMonster.Skeleton;
                break;
            }
            default:
            {
                yield return SMonster.Sludge;
                yield return SMonster.LavaCrab;
                if (mineLevel > 90) yield return SMonster.SquidKid;
                break;
            }
        }
    }

    private IEnumerable<string> GetModMineShaftMonsters()
    {
        var mineLevel = Utility.GetAllPlayerDeepestMineLevel();

        switch (mineLevel)
        {
            case <= 40:
            {
                yield return SMonster.Bug;
                if (mineLevel > 10)
                {
                    yield return SMonster.Grub;
                    yield return SMonster.Fly;
                }
                if (mineLevel > 30)
                {
                    yield return SMonster.Bat;
                    yield return SMonster.StoneGolem;
                }
                break;
            }
            case <= 80:
            {
                yield return SMonster.FrostBat;
                break;
            }
            default:
            {
                yield return SMonster.LavaBat;
                yield return SMonster.ShadowBrute;
                yield return SMonster.ShadowShaman;
                yield return SMonster.MetalHead;
                break;
            }
        }

        if (mineLevel > 120)
        {
            yield return SMonster.BigSlime;
            yield return SMonster.Mummy;
            yield return SMonster.Serpent;
            yield return SMonster.CarbonGhost;
            yield return SMonster.PepperRex;
            if (mineLevel > 145) yield return SMonster.IridiumCrab;
            if (mineLevel > 170) yield return SMonster.IridiumBat;
        }
    }

    private IEnumerable<string> GetModVolcanoDungeonMonsters()
    {
        if (!Utility.doesAnyFarmerHaveMail("addedParrotBoy")) yield break;

        yield return SMonster.DwarvishSentry;
        yield return SMonster.FalseMagmaCap;
        yield return SMonster.HotHead;
        yield return SMonster.LavaLurk;
        yield return SMonster.MagmaDuggy;
        yield return SMonster.MagmaSparker;
        yield return SMonster.MagmaSprite;
        yield return SMonster.TigerSlime;
    }
}