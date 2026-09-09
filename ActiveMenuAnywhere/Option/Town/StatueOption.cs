using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class StatueOption : BaseOption
{
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
