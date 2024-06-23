using StardewModdingAPI;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

internal abstract class BaseHandler : IHandler
{
    protected readonly IModHelper Helper;
    protected readonly ModConfig Config;

    protected BaseHandler(IModHelper helper, ModConfig config)
    {
        this.Helper = helper;
        this.Config = config;
    }

    public abstract void Init();
}