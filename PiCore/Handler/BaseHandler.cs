using StardewModdingAPI;

namespace weizinai.StardewValleyMod.PiCore.Handler;

public abstract class BaseHandler : IHandler
{
    protected readonly IModHelper helper;

    protected BaseHandler(IModHelper helper)
    {
        this.helper = helper;
    }

    public abstract void Apply();

    public virtual void Clear() { }
}
