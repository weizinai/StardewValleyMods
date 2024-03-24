using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ActiveMenuAnywhere.Framework;

public class ModConfig
{
    public KeybindList MenuKey { get; set; } = new(SButton.L);
    public MenuTab DefaultMeanTab { get; set; } = MenuTab.Town;
}