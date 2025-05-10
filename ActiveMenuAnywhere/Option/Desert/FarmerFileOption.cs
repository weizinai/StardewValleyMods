using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class FarmerFileOption : BaseOption
{
    public FarmerFileOption()
        : base(I18n.UI_Option_FarmerFile(), TextureManager.Instance.DesertTexture, GetSourceRectangle(3), OptionId.FarmerFile) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard;
    }

    public override void Apply()
    {
        Game1.currentLocation.farmerFile();
    }
}