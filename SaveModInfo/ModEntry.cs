using StardewModdingAPI;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.SaveModInfo.Handler;
using weizinai.StardewValleyMod.SaveModInfo.Record;

namespace weizinai.StardewValleyMod.SaveModInfo;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger<ModEntry>.Init(this);
        this.InitHandler();
    }

    private void InitHandler()
    {
        // 记录与差异的存取边界由两个处理器共用：写入新记录要能作废叠层那边的缓存
        var store = new RecordStore(this.Helper);

        new RecordModInfoHandler(this.Helper, store).Apply();
        new LoadGameOverlayHandler(this.Helper, store).Apply();
    }
}
