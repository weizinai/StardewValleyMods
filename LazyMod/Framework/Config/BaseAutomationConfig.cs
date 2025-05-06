namespace weizinai.StardewValleyMod.LazyMod.Framework.Config;

internal class BaseAutomationConfig
{
    public bool IsEnable { get; set; }
    public int Range { get; set; }

    public BaseAutomationConfig(int range)
    {
        this.Range = range;
    }
}