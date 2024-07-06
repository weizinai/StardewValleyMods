using StardewValley;

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal static class PanScreenHelper
{
    public static void PanScreen(int moveSpeed, int moveThreshold)
    {
        PanScreenByMouse(moveSpeed, moveThreshold);
        PanScreenByKey(moveSpeed);
    }

    private static void PanScreenByMouse(int moveSpeed, int moveThreshold)
    {
        var mouseX = Game1.getOldMouseX(false);
        var mouseY = Game1.getOldMouseY(false);
        
        if (mouseX < moveThreshold)
            Game1.panScreen(-moveSpeed, 0);
        else if (mouseX - Game1.viewport.Width >= -moveThreshold)
            Game1.panScreen(moveSpeed, 0);
        
        if (mouseY < moveThreshold)
            Game1.panScreen(0, -moveSpeed);
        else if (mouseY - Game1.viewport.Height >= -moveThreshold)
            Game1.panScreen(0, moveSpeed);
    }

    private static void PanScreenByKey(int moveSpeed)
    {
        var pressedKeys = Game1.oldKBState.GetPressedKeys();
        
        foreach (var key in pressedKeys)
        {
            if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                Game1.panScreen(0, moveSpeed);
            else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                Game1.panScreen(moveSpeed, 0);
            else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                Game1.panScreen(0, -moveSpeed);
            else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                Game1.panScreen(-moveSpeed, 0);    
        }
    }
}