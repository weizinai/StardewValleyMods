using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

internal class ModConfig
{
    public KeybindList MenuKey { get; set; } = new(SButton.L);
    public MenuTabID DefaultMeanTabID { get; set; } = MenuTabID.Town;
}