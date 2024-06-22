using weizinai.StardewValleyMod.Common.Patcher;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.BetterCabin.Framework.UI;

namespace weizinai.StardewValleyMod.BetterCabin.Patches;

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
            RequireMethod<Building>(nameof(Building.draw)),
            postfix: GetHarmonyMethod(nameof(DrawPostfix))
        );
    }

    private static void DrawPostfix(Building __instance, SpriteBatch b)
    {
        if (__instance.GetIndoors() is Cabin cabin && !cabin.owner.isUnclaimedFarmhand)
        {
            if (config.CabinOwnerNameTag)
            {
                nameTag = new CabinOwnerNameBox(__instance, cabin, config);
                nameTag.Draw(b);
            }

            if (config.TotalOnlineTime.Enable)
            {
                totalOnlineTimeTag = new TotalOnlineTimeBox(__instance, cabin, config);
                totalOnlineTimeTag.Draw(b);
            }

            if (config.LastOnlineTime.Enable && !Game1.player.team.playerIsOnline(cabin.owner.UniqueMultiplayerID))
            {
                lastOnlineTimeTag = new LastOnlineTimeBox(__instance, cabin, config);
                lastOnlineTimeTag.Draw(b);
            }
        }
    }
}