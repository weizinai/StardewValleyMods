namespace weizinai.StardewValleyMod.LazyMod.Framework;

internal interface IAutomationHandlerWithDayChanged : IAutomationHandler
{
    public void OnDayStarted();
    public void OnDayEnding();
}