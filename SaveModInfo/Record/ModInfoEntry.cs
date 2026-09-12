namespace weizinai.StardewValleyMod.SaveModInfo.Record;

/// <summary>记录里的单个模组条目：显示名与版本号。</summary>
internal class ModInfoEntry
{
    /// <summary>模组显示名（清单里的 <c>Name</c>）。</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>模组版本号（清单里的 <c>Version</c>）。</summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>构造空条目（JSON 反序列化用）。</summary>
    public ModInfoEntry() { }

    /// <summary>构造条目。</summary>
    /// <param name="name">模组显示名。</param>
    /// <param name="version">模组版本号。</param>
    public ModInfoEntry(string name, string version)
    {
        this.Name = name;
        this.Version = version;
    }
}
