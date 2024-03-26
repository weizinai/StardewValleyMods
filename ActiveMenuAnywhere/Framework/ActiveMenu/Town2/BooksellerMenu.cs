using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class BooksellerMenu : BaseActiveMenu
{
    public BooksellerMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Utility.getDaysOfBooksellerThisSeason().Contains(Game1.dayOfMonth))
            BookSeller();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }

    private void BookSeller()
    {
        if (Game1.player.mailReceived.Contains("read_a_book"))
        {
            var options = new List<Response>
            {
                new("Buy", Game1.content.LoadString("Strings\\1_6_Strings:buy_books")),
                new("Trade", Game1.content.LoadString("Strings\\1_6_Strings:trade_books")),
                new("Leave", Game1.content.LoadString("Strings\\1_6_Strings:Leave"))
            };
            Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:books_welcome"),
                options.ToArray(), "Bookseller");
        }
        else
        {
            Utility.TryOpenShopMenu("Bookseller", null, true);
        }
    }
}