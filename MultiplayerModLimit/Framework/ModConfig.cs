namespace weizinai.StardewValleyMod.MultiplayerModLimit.Framework;

internal class ModConfig
{
    public bool EnableMod { get; set; } = true;
    public int KickPlayerDelayTime { get; set; } = 3;
    public bool RequireSMAPI { get; set; } = true;
    public LimitMode LimitMode { get; set; } = LimitMode.WhiteListMode;
    public string AllowedModListSelected { get; set; } = "Default";
    public string RequiredModListSelected { get; set; } = "Default";
    public string BannedModListSelected { get; set; } = "Default";

    public Dictionary<string, List<string>> AllowedModList { get; set; } = new() { { "Default", new List<string>() } };
    public Dictionary<string, List<string>> RequiredModList { get; set; } = new() { { "Default", new List<string>() } };
    public Dictionary<string, List<string>> BannedModList { get; set; } = new() { { "Default", new List<string>() } };
}