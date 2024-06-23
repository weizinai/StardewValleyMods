using StardewModdingAPI;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Framework;

internal abstract class BaseHandler : IHandler
{
    protected readonly ModConfig Config;
    protected readonly IModHelper Helper;

    protected BaseHandler(ModConfig config, IModHelper helper)
    {
        this.Config = config;
        this.Helper = helper;
    }

    public abstract void Init();
    public abstract void Clear();
}