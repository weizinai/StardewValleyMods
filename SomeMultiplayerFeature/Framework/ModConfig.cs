using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace SomeMultiplayerFeature.Framework;

public class ModConfig
{
    public bool ShowShopInfo { get; set; } = true;

    // public bool ShowModInfo { get; set; } = true;
    public KeybindList ShowModInfoKeybind { get; set; } = new(SButton.L);
    public KeybindList SetAllPlayerReadyKeybind { get; set; } = new(SButton.K);
}