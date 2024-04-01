using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class FarmerFileOption : BaseOption
{
    private readonly IModHelper helper;

    public FarmerFileOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture, sourceRect, I18n.Option_FarmerFile())
    {
        this.helper = helper;
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard)
            helper.Reflection.GetMethod(new GameLocation(), "farmerFile").Invoke();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}