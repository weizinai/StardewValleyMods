using System.Reflection;
using Common;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace ActiveMenuAnywhere.Framework.Options.RSV;

public class PaulaOption : BaseOption
{
    private readonly IModHelper helper;
    
    public PaulaOption(Rectangle sourceRect, IModHelper helper) : base(I18n.Option_Paula(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var targetDllPath = CommonHelper.GetDllPath(helper, "RidgesideVillage.dll");
        var assembly = Assembly.LoadFrom(targetDllPath);
        var lanHouse = assembly.GetType("RidgesideVillage.PaulaClinic");
        lanHouse?.GetMethod("ClinicChoices", BindingFlags.NonPublic|BindingFlags.Static)?.Invoke(null, null);
    }
}