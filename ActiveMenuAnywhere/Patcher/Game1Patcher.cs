using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Patcher;

internal class Game1Patcher : BasePatcher
{
    private static Game1Patcher instance = null!;
    private readonly IModHelper helper;

    public Game1Patcher(IModHelper helper)
    {
        if (instance != null)
        {
            throw new InvalidOperationException($"{nameof(Game1Patcher)} already initialized.");
        }

        this.helper = helper;
        instance = this;
    }

    public override void Apply(Harmony harmony)
    {
        this.Patch<Game1>(harmony, nameof(Game1.ShowTelephoneMenu), PatchKind.Prefix, nameof(ShowTelephoneMenuPrefix));
    }

    private static bool ShowTelephoneMenuPrefix()
    {
        if (!ModConfig.Instance.OpenMenuByTelephone)
        {
            return true;
        }

        Game1.activeClickableMenu = new AMAMenu(ModConfig.Instance.DefaultMenuTabId, instance.helper);

        return false;
    }
}
