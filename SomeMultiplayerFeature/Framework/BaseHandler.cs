using StardewModdingAPI;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

internal abstract class BaseHandler : IHandler
{
    protected readonly IModHelper Helper;
    protected readonly ModConfig Config;

    protected BaseHandler(IModHelper helper, ModConfig config)
    {
        Helper = helper;
        Config = config;
    }

    public abstract void Init();
}