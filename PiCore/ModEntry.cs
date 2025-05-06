using StardewModdingAPI;
using weizinai.StardewValleyMod.Common;

namespace weizinai.StardewValleyMod.PiCore;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Logger.Init(this.Monitor);
    }
}