using System.Reflection;

namespace Common.Integration;

public class RSVIntegration
{
    private const string DllPath = @"C:\Projects\StardewValleyMods\Common\ModDll\RidgesideVillage.dll";

    public static Type? GetType(string name)
    {
        var assembly = Assembly.LoadFrom(DllPath);
        return assembly.GetType(name);
    }
}