namespace weizinai.StardewValleyMod.SaveModInfo.Record;

/// <summary>一条「已更新」的模组变化：显示名 + 记录时的版本与当前版本（界面展示为「旧 -&gt; 新」）。</summary>
/// <param name="Name">模组显示名（取当前已安装的那份名字）。</param>
/// <param name="OldVersion">记录时的版本号。</param>
/// <param name="NewVersion">当前版本号。</param>
internal sealed record ModVersionChange(string Name, string OldVersion, string NewVersion);
