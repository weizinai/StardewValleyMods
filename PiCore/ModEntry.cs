using StardewModdingAPI;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.PiCore;

public class ModEntry : Mod
{
    public static bool IsSVELoaded { get; private set; }

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Logger<ModEntry>.Init(this);
        // 统一订阅多人消息，接收侧按发送方模组路由日志与 HUD
        helper.Events.Multiplayer.ModMessageReceived += HudBroadcaster.OnModMessageReceived;

        IsSVELoaded = this.Helper.ModRegistry.IsLoaded("FlashShifter.SVECode");
    }
}
