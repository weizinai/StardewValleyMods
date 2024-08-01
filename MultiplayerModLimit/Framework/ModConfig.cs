namespace weizinai.StardewValleyMod.MultiplayerModLimit.Framework;

internal class ModConfig
{
    // 一般设置
    public bool EnableMod { get; set; } = true;
    public int KickPlayerDelayTime { get; set; } = 3;
    public bool SendSMAPIInfo { get; set; } = true;

    // 限制设置
    public bool RequireSMAPI { get; set; } = true;
    public LimitMode LimitMode { get; set; } = LimitMode.WhiteListMode;

    // 选择的模组列表设置
    public string AllowedModListSelected { get; set; } = "Default";
    public string RequiredModListSelected { get; set; } = "Default";
    public string BannedModListSelected { get; set; } = "Default";

    public Dictionary<string, List<string>> AllowedModList { get; set; } = new() { { "Default", new List<string>() } };
    public Dictionary<string, List<string>> RequiredModList { get; set; } = new() { { "Default", new List<string>() } };
    public Dictionary<string, List<string>> BannedModList { get; set; } = new() { { "Default", new List<string>() } };
}