using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu.Desert;

public class FarmerFileActiveMenu: BaseActiveMenu
{
    private readonly IModHelper helper;

    public FarmerFileActiveMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture, sourceRect)
    {
        this.helper = helper;
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault")  && Game1.player.hasClubCard)
            helper.Reflection.GetMethod(new GameLocation(), "farmerFile").Invoke();
        else
            Game1.drawObjectDialogue("不好意思，你还不能进入赌场");
        
    }
}