namespace weizinai.StardewValleyMod.TestMod.Config;

internal class ToggleValueConfig<T>
{
    public bool IsEnabled { get; set; }
    public T Value { get; set; }

    public ToggleValueConfig(bool isEnabled, T value)
    {
        this.IsEnabled = isEnabled;
        this.Value = value;
    }
}
