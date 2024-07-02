using StardewModdingAPI;

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal abstract class BaseHandler : IHandler
{
    protected readonly IModHelper Helper;
    protected readonly ModConfig Config = ModEntry.Config;

    protected BaseHandler(IModHelper helper)
    {
        this.Helper = helper;
    }

    public abstract void Init();
}