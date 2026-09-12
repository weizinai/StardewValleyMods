using StardewModdingAPI;
using weizinai.StardewValleyMod.PiCore.Hud;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.PiCore;

public class ModEntry : Mod
{
    public static bool IsSVELoaded { get; private set; }

    public override void Entry(IModHelper helper)
    {
        // 统一订阅多人消息：两个接收侧各认自己的消息类型，日志按发送方模组路由、HUD 直接显示
        helper.Events.Multiplayer.ModMessageReceived += LogReceiver.OnModMessageReceived;
        helper.Events.Multiplayer.ModMessageReceived += HudReceiver.OnModMessageReceived;

        IsSVELoaded = this.Helper.ModRegistry.IsLoaded("FlashShifter.SVECode");
    }
}
