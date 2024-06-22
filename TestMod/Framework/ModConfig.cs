using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.TestMod.Framework;

public class ModConfig
{
    public KeybindList Key { get; set; } = new(SButton.L);
    
    public int MineShaftMap { get; set; } = 40;
    public int VolcanoDungeonMap { get; set; } = 46;
    
    public int RandomInt { get; set; } = 0;
    public bool RandomBool { get; set; } = false;
}