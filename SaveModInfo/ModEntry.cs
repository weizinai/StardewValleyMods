using StardewModdingAPI;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.SaveModInfo.Handler;
using weizinai.StardewValleyMod.SaveModInfo.Patcher;

namespace weizinai.StardewValleyMod.SaveModInfo;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger.Init(this.Monitor);
        this.InitHandler();
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this.ModManifest.UniqueID, new LoadGameMenuPatcher(), new SaveFileSlotPatcher());
    }

    private void InitHandler()
    {
        var handlers = new IHandler[]
        {
            new RecordModInfoHandler(this.Helper),
            new CheckModInfoHandler(this.Helper)
        };

        foreach (var handler in handlers) handler.Apply();
    }
}