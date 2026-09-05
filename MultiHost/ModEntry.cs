using StardewModdingAPI;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.MultiHost.Patcher;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.MultiHost;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        Logger.Init(this.Monitor);
        // 注册补丁
        HarmonyPatcher.Apply(this.ModManifest.UniqueID, new LidgrenServerPatcher());
    }
}