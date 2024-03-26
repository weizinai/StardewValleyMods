using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class MarlonMenu : BaseActiveMenu
{
    private readonly IModHelper helper;

    public MarlonMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture, sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("guildMember"))
            helper.Reflection.GetMethod(new GameLocation(), "adventureShop").Invoke();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}