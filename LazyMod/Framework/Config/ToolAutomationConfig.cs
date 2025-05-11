namespace weizinai.StardewValleyMod.LazyMod.Framework.Config;

public class ToolAutomationConfig : BaseAutomationConfig
{
    public bool FindToolFromInventory;

    public ToolAutomationConfig(int range, bool findToolFromInventory) : base(range)
    {
        this.FindToolFromInventory = findToolFromInventory;
    }
}