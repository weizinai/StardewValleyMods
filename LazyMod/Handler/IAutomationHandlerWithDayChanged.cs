namespace weizinai.StardewValleyMod.LazyMod.Handler;

public interface IAutomationHandlerWithDayChanged : IAutomationHandler
{
    public void OnDayStarted();

    public void OnDayEnding();
}