using StardewModdingAPI;
using weizinai.StardewValleyMod.Common;

namespace weizinai.StardewValleyMod.PiCore;

public class ModEntry : Mod
{
    public static bool IsSVELoaded { get; private set; }

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Logger.Init(this.Monitor);

        IsSVELoaded = this.Helper.ModRegistry.IsLoaded("FlashShifter.SVECode");
    }
}