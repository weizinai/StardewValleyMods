namespace LazyMod.Framework.Config;

internal class BaseConfig
{
    public bool IsEnable { get; set; } 
    public int Range { get; set; }

    public BaseConfig(int range = 0)
    {
        Range = range;
    }
}