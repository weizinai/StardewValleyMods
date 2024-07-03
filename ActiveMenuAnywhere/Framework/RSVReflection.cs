using System.Reflection;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

internal static class RSVReflection
{
    public static Type GetRSVType(string typeName)
    {
        var type = Type.GetType($"{typeName}, RidgesideVillage");
        if (type is null) throw new ArgumentException($"Could not find type {typeName} in RidgesideVillage assembly.");
        return type;
    }

    public static MethodInfo GetRSVPrivateStaticMethod(string typeName, string methodName)
    {
        var type = GetRSVType(typeName);
        var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        if (method is null) throw new ArgumentException($"Could not find method {methodName} in type {typeName}.");
        return method;
    }
}