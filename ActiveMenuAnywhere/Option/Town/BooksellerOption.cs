using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class BooksellerOption : BaseOption
{
    public BooksellerOption(Rectangle sourceRect) :
        base(I18n.UI_Option_Bookseller(), sourceRect) { }

    public override bool IsEnable()
    {
        return Utility.getDaysOfBooksellerThisSeason().Contains(Game1.dayOfMonth);
    }

    public override void Apply()
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