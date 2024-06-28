using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

internal class StatueOption : BaseOption
{
    public StatueOption(Rectangle sourceRect) :
        base(I18n.Option_Statue(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.hasRustyKey)
            this.Statue();
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