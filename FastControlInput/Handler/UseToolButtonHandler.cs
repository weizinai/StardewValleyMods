using Microsoft.Xna.Framework.Input;
using StardewValley;
using weizinai.StardewValleyMod.FastControlInput.Framework;

namespace weizinai.StardewValleyMod.FastControlInput.Handler;

internal class UseToolButtonHandler : BaseInputHandler
{
    public UseToolButtonHandler(float multiplier) : base(multiplier) { }

    public override bool IsEnable()
    {
        var currentKeyboardState = Game1.GetKeyboardState();
        var currentMouseState = Game1.input.GetMouseState();
        var currentGamePadState = Game1.input.GetGamePadState();

        return Game1.isOneOfTheseKeysDown(currentKeyboardState, Game1.options.useToolButton)
               || currentMouseState.LeftButton == ButtonState.Pressed
               || Game1.options.gamepadControls && currentGamePadState.IsButtonDown(Buttons.X);
    }

    public override void Update()
    {
        Game1.mouseClickPolling += (int)(Game1.currentGameTime.ElapsedGameTime.Milliseconds * (this.Multiplier - 1));
    }
}