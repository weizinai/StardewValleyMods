using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Config;

internal class ModConfig
{
    public static ModConfig Instance { get; set; } = null!;

    public KeybindList MenuKey { get; set; } = new(SButton.L);
    public bool OpenMenuByTelephone { get; set; }
    public string DefaultMenuTabId { get; set; } = OptionCatalog.TownTabId;
    public bool ProgressMode { get; set; } = true;
    public KeybindList FavoriteKey { get; set; } = new(SButton.LeftAlt);
    public List<string> FavoriteMenus { get; set; } = new();
}
