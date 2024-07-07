using StardewModdingAPI;

namespace weizinai.StardewValleyMod.Common.Handler;

internal abstract class BaseHandlerWithConfig<TConfig> : BaseHandler where TConfig : new()
{
    protected readonly TConfig Config;

    protected BaseHandlerWithConfig(IModHelper helper, TConfig config) : base(helper)
    {
        this.Config = config;
    }
}