using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.ReadyCheckKick.Framework;
using weizinai.StardewValleyMod.ReadyCheckKick.Handler;
using weizinai.StardewValleyMod.ReadyCheckKick.Patcher;

namespace weizinai.StardewValleyMod.ReadyCheckKick;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        I18n.Init(this.Helper.Translation);
        Logger<ModEntry>.Init(this);
        ModConfig.Init(helper);

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        HarmonyPatcher.Apply(this.ModManifest.UniqueID, new SaveGameMenuPatcher(helper.Reflection));

        new ReadyCheckDialogueHandler(helper).Apply();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            new GenericModConfigMenuForReadyCheckKick(),
            () => ModConfig.Instance,
            config => ModConfig.Instance = config
        );
    }
}
