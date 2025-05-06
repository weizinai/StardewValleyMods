using System.Collections.Generic;
using StardewValley;
using StardewValley.Extensions;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Repository;

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
                yield return MonsterRepository.GreenSlime;
                if (mineLevel > 10) yield return MonsterRepository.RockCrab;
                if (mineLevel > 30) yield return MonsterRepository.Duggy;
                break;
            }
            case <= 80:
            {
                yield return MonsterRepository.FrostJelly;
                yield return MonsterRepository.DustSpirit;
                if (mineLevel > 50) yield return MonsterRepository.Ghost;
                if (mineLevel > 70) yield return MonsterRepository.Skeleton;
                break;
            }
            default:
            {
                yield return MonsterRepository.Sludge;
                yield return MonsterRepository.LavaCrab;
                if (mineLevel > 90) yield return MonsterRepository.SquidKid;
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
                yield return MonsterRepository.Bug;
                if (mineLevel > 10)
                {
                    yield return MonsterRepository.Grub;
                    yield return MonsterRepository.Fly;
                }
                if (mineLevel > 30)
                {
                    yield return MonsterRepository.Bat;
                    yield return MonsterRepository.StoneGolem;
                }
                break;
            }
            case <= 80:
            {
                yield return MonsterRepository.FrostBat;
                break;
            }
            default:
            {
                yield return MonsterRepository.LavaBat;
                yield return MonsterRepository.ShadowBrute;
                yield return MonsterRepository.ShadowShaman;
                yield return MonsterRepository.MetalHead;
                break;
            }
        }

        if (mineLevel > 120)
        {
            yield return MonsterRepository.BigSlime;
            yield return MonsterRepository.Mummy;
            yield return MonsterRepository.Serpent;
            yield return MonsterRepository.CarbonGhost;
            yield return MonsterRepository.PepperRex;
            if (mineLevel > 145) yield return MonsterRepository.IridiumCrab;
            if (mineLevel > 170) yield return MonsterRepository.IridiumBat;
        }
    }

    private IEnumerable<string> GetModVolcanoDungeonMonsters()
    {
        if (!Utility.doesAnyFarmerHaveMail("addedParrotBoy")) yield break;

        yield return MonsterRepository.DwarvishSentry;
        yield return MonsterRepository.FalseMagmaCap;
        yield return MonsterRepository.HotHead;
        yield return MonsterRepository.LavaLurk;
        yield return MonsterRepository.MagmaDuggy;
        yield return MonsterRepository.MagmaSparker;
        yield return MonsterRepository.MagmaSprite;
        yield return MonsterRepository.TigerSlime;
    }
}