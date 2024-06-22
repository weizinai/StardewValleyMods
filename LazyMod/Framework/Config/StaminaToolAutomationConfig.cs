namespace weizinai.StardewValleyMod.LazyMod.Framework.Config;

internal class StaminaToolAutomationConfig : ToolAutomationConfig
{
    public float StopStamina;
    
    public StaminaToolAutomationConfig(int range, float stopStamina, bool findToolFromInventory) 
        : base(range, findToolFromInventory)
    {
        StopStamina = stopStamina;
    }
}