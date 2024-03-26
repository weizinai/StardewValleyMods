using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class StatueMenu : BaseActiveMenu
{
    public StatueMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.hasRustyKey)
            Statue();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }

    private void Statue()
    {
        var location = Game1.currentLocation;
        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatue"),
            location.createYesNoResponses(), "dogStatue");
    }
}