using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.BetterCabin.Framework.UI;
using weizinai.StardewValleyMod.Common.Extension;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin.Patcher;

internal class BuildingPatcher : BasePatcher
{
    private static ModConfig config = null!;
    private static CabinOwnerNameBox nameTag = null!;
    private static TotalOnlineTimeBox totalOnlineTimeTag = null!;
    private static LastOnlineTimeBox lastOnlineTimeTag = null!;

    public BuildingPatcher(ModConfig config)
    {
        BuildingPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<Building>(nameof(Building.draw)),
            postfix: this.GetHarmonyMethod(nameof(DrawPostfix))
        );
    }

    private static void DrawPostfix(Building __instance, SpriteBatch b)
    {
        if (__instance.IsCabinWithOwner(out var cabin))
        {
            // 小屋主人名字标签
            if (config.CabinOwnerNameTag)
            {
                nameTag = new CabinOwnerNameBox(__instance, cabin, config);
                nameTag.Draw(b);
            }

            // 总在线时间标签
            if (config.TotalOnlineTime.Enable)
            {
                totalOnlineTimeTag = new TotalOnlineTimeBox(__instance, cabin, config);
                totalOnlineTimeTag.Draw(b);
            }

            // 上次在线时间标签
            if (config.LastOnlineTime.Enable && !Game1.player.team.playerIsOnline(cabin.owner.UniqueMultiplayerID))
            {
                lastOnlineTimeTag = new LastOnlineTimeBox(__instance, cabin, config);
                lastOnlineTimeTag.Draw(b);
            }
        }
    }
}