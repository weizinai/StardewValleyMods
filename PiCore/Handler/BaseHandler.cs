using StardewModdingAPI;

namespace weizinai.StardewValleyMod.PiCore.Handler;

public abstract class BaseHandler : IHandler
{
    protected readonly IModHelper Helper;

    protected BaseHandler(IModHelper helper)
    {
        this.Helper = helper;
    }

    public abstract void Apply();

    public virtual void Clear() { }
}