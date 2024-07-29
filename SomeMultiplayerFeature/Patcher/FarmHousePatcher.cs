using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class FarmHousePatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<FarmHouse>("AddStarterGiftBox"),
            postfix: this.GetHarmonyMethod(nameof(AddStarterGiftBoxPostfix))
        );

        Log.Info("为初始种子包添加三种树种各10个");
    }

    // 为初始种子包添加三种树种各10个
    private static void AddStarterGiftBoxPostfix(Farm farm, FarmHouse __instance)
    {
        if (!farm.TryGetMapPropertyAs("FarmHouseStarterSeedsPosition", out Vector2 tile, required: false))
        {
            tile = Game1.whichFarm switch
            {
                1 or 2 or 4 => new Vector2(4f, 7f),
                3 => new Vector2(2f, 9f),
                6 => new Vector2(8f, 6f),
                _ => new Vector2(3f, 7f)
            };
        }

        __instance.Objects.TryGetValue(tile, out var obj);
        if (obj is Chest chest)
        {
            chest.Items.AddRange(new List<Item>
            {
                ItemRegistry.Create("(O)309", 10),
                ItemRegistry.Create("(O)310", 10),
                ItemRegistry.Create("(O)311", 10)
            });
        }
    }
}