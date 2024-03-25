using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class FarmerFileMenu : BaseActiveMenu
{
    private readonly IModHelper helper;

    public FarmerFileMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture, sourceRect)
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