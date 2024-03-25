using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu.Mountain;

public class MarlonActiveMenu :BaseActiveMenu
{
    private IModHelper helper;
    
    public MarlonActiveMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture, sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("guildMember"))
            helper.Reflection.GetMethod(new GameLocation(), "adventureShop").Invoke();
        else
            Game1.drawObjectDialogue("不好意思，你还没有加入冒险家协会");
    }
}