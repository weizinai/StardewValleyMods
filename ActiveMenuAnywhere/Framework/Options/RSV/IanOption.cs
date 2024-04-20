using System.Reflection;
using Common;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace ActiveMenuAnywhere.Framework.Options;

public class IanOption : BaseOption
{
    private readonly IModHelper helper;
    
    public IanOption(Rectangle sourceRect, IModHelper helper) : base(I18n.Option_Ian(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var targetDllPath = CommonHelper.GetDllPath(helper, "RidgesideVillage.dll");
        var assembly = Assembly.LoadFrom(targetDllPath);
        var lanHouse = assembly.GetType("RidgesideVillage.IanShop");
        lanHouse?.GetMethod("IanCounterMenu", BindingFlags.NonPublic|BindingFlags.Static)?.Invoke(null, null);
    }
}