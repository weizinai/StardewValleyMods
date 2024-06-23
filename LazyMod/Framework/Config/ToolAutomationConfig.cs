namespace weizinai.StardewValleyMod.LazyMod.Framework.Config;

internal class ToolAutomationConfig : BaseAutomationConfig
{
    public bool FindToolFromInventory;

    public ToolAutomationConfig(int range, bool findToolFromInventory): base(range)
    {
        this.FindToolFromInventory = findToolFromInventory;
    }
}