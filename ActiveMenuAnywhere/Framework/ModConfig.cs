using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ActiveMenuAnywhere.Framework;

public class ModConfig
{
    public KeybindList MenuKey { get; set; } = new(SButton.L);
    public MenuTabID DefaultMeanTabID { get; set; } = MenuTabID.Town1;
}