using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using weizinai.StardewValleyMod.Common.Log;

namespace TestMod;

public class ModEntry : Mod
{
    private readonly KeybindList testKet = new(SButton.O);
    
    public override void Entry(IModHelper helper)
    {
        Log.Init(this.Monitor);
        
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.testKet.JustPressed())
            Log.Info(Game1.player.slotName);
    }
}