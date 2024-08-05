using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class MarlonOption : BaseOption
{
    public MarlonOption()
        : base(I18n.UI_Option_Marlon(), TextureManager.Instance.MountainTexture, GetSourceRectangle(3), OptionId.Marlon) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("guildMember");
    }

    public override void Apply()
    {
        if (Game1.player.itemsLostLastDeath.Count > 0)
        {
            var options = new List<Response>
            {
                new("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")),
                new("Recovery", Game1.content.LoadString("Strings\\Locations:AdventureGuild_ItemRecovery")),
                new("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave"))
            };
            Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:AdventureGuild_Greeting"),
                options.ToArray(), "adventureGuild");
        }
        else
        {
            Utility.TryOpenShopMenu("AdventureShop", "Marlon");
        }
    }
}