using BetterCabin.Framework;
using BetterCabin.Framework.Config;
using BetterCabin.Framework.UI;
using Common.Patcher;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace BetterCabin.Patches;

internal class BuildingPatcher : BasePatcher
{
    private static ModConfig config = null!;
    private static CabinOwnerNameBox box = null!;

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
        if (!config.CabinOwnerNameTag) return;
        
        if (__instance.GetIndoors() is Cabin cabin && !cabin.owner.isUnclaimedFarmhand)
        {
            box = new CabinOwnerNameBox(__instance, cabin, config);
            box.Draw(b);
        }
    }
}