using StardewModdingAPI;

namespace SaveModInfo.Framework;

internal abstract class BaseHandler : IHandler
{
    protected readonly IModHelper Helper;

    protected BaseHandler(IModHelper helper)
    {
        this.Helper = helper;
    }

    public abstract void Init();
}