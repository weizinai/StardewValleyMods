using BetterCabin.Framework.Config;
using BetterCabin.Framework.UI;
using Common.Patcher;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace BetterCabin.Patches;

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

            if (config.LastOnlineTime.Enable)
            {
                lastOnlineTimeTag = new LastOnlineTimeBox(__instance, cabin, config);
                lastOnlineTimeTag.Draw(b);
            }
        }
    }
}