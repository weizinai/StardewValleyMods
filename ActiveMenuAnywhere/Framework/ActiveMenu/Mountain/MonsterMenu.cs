using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class MonsterMenu : BaseActiveMenu
{
    private IModHelper helper;

    public MonsterMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture,
        sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("guildMember"))
            helper.Reflection.GetMethod(new AdventureGuild(), "showMonsterKillList").Invoke();
        else
            Game1.drawObjectDialogue("不好意思，你还没有加入冒险家协会");
    }
}