using System;
using System.Reflection;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Helper;

internal static class RSVReflection
{
    private static Type GetRSVType(string typeName)
    {
        var type = Type.GetType($"{typeName}, RidgesideVillage");

        return type ?? throw new ArgumentException($"Could not find type {typeName} in RidgesideVillage assembly.");
    }

    public static MethodInfo GetRSVPrivateStaticMethod(string typeName, string methodName)
    {
        var type = GetRSVType(typeName);
        var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);

        return method ?? throw new ArgumentException($"Could not find method {methodName} in type {typeName}.");
    }
}
