using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class StatueOption : BaseOption
{
    public StatueOption()
        : base(I18n.UI_Option_Statue(), TextureManager.Instance.TownTexture, GetSourceRectangle(10), OptionId.Statue) { }

    public override bool IsEnable()
    {
        return Game1.player.hasRustyKey;
    }

    public override void Apply()
    {
        var location = Game1.currentLocation;

        location.createQuestionDialogue(
            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatue"),
            location.createYesNoResponses(),
            "dogStatue"
        );
    }
}