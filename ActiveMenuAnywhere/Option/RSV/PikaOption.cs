using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class PikaOption : BaseOption
{
    public PikaOption()
        : base(I18n.UI_Option_Pika(),TextureManager.Instance.RSVTexture, GetSourceRectangle(7), OptionId.Pika) { }

    public override void Apply()
    {
        var options = new List<Response>
        {
            new("Shop", I18n.UI_PikaOption_Shop()),
            new("RecipeShop", I18n.UI_PikaOption_RecipeShop()),
            new("Leave", I18n.UI_BaseOption_Leave())
        };
        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), this.AfterDialogueBehavior);
    }

    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "Shop":
                Utility.TryOpenShopMenu("RSVPikaShop", "Pika");
                break;
            case "RecipeShop":
                Utility.TryOpenShopMenu("RSVPikaRecipes", "Pika");
                break;
            case "Leave":
                Game1.exitActiveMenu();
                Game1.player.forceCanMove();
                break;
        }
    }
}