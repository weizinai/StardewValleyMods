using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace ActiveMenuAnywhere.Framework.Options;

internal class PaulaOption : BaseOption
{
    private readonly IModHelper helper;

    public PaulaOption(Rectangle sourceRect, IModHelper helper) : base(I18n.Option_Paula(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        // var lanHouse = RSVIntegration.GetType("RidgesideVillage.PaulaClinic");
        // lanHouse?.GetMethod("ClinicChoices", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
    }
}