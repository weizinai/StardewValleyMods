using HarmonyLib;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StardewModdingAPI;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Patcher;

internal class Game1Patcher : BasePatcher
{
    private static ModConfig config = null!;
    private static IModHelper helper = null!;

    public Game1Patcher(ModConfig config, IModHelper helper)
    {
        Game1Patcher.config = config;
        Game1Patcher.helper = helper;
    }
    
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            this.RequireMethod<Game1>(nameof(Game1.ShowTelephoneMenu)), 
            this.GetHarmonyMethod(nameof(ShowTelephoneMenuPrefix))
        );
    }

    private static bool ShowTelephoneMenuPrefix()
    {
        if (!config.OpenMenuByTelephone) return true;

        Game1.activeClickableMenu = new AMAMenu(config.DefaultMeanTabId, config, helper);

        return false;
    }
}