namespace weizinai.StardewValleyMod.LazyMod.Framework.Config;

public class BaseAutomationConfig
{
    public bool IsEnable { get; set; }
    public int Range { get; set; }

    public BaseAutomationConfig(int range)
    {
        this.Range = range;
    }
}