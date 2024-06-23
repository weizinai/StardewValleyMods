using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Framework;

internal abstract class BaseHandler : IHandler
{
    protected readonly ModConfig Config;

    protected BaseHandler(ModConfig config)
    {
        this.Config = config;
    }

    public abstract void Init();
}