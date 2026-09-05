namespace weizinai.StardewValleyMod.TestMod.Framework;

internal class SingleValueConfig<T>
{
    public bool IsEnabled { get; set; }
    public T Value { get; set; }

    public SingleValueConfig(bool isEnabled, T value)
    {
        this.IsEnabled = isEnabled;
        this.Value = value;
    }
}