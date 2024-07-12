using SaveModInfo.Handler;
using SaveModInfo.Patcher;
using StardewModdingAPI;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;

namespace SaveModInfo;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        this.InitHandler();
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new LoadGameMenuPatcher(), new SaveFileSlotPatcher());
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