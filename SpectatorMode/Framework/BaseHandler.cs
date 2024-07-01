using StardewModdingAPI;

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal abstract class BaseHandler : IHandler
{
    protected readonly ModConfig Config = ModEntry.Instance.Config;
    protected readonly IModHelper Helper = ModEntry.Instance.Helper;

    public abstract void Init();
}