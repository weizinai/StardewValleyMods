using System.Reflection;
using HarmonyLib;

namespace Common.Patch;

public abstract class BasePatcher : IPatcher
{
    public abstract void Patch(Harmony harmony);
    
    protected MethodInfo RequireMethod<T>(string name, Type[]? parameters = null)
    {
        return AccessTools.Method(typeof(T), name, parameters);
    }
    
    protected ConstructorInfo RequireConstructor<T>(Type[]? parameters = null)
    {
        return AccessTools.Constructor(typeof(T), parameters);
    }
    
    protected HarmonyMethod GetHarmonyMethod(string name)
    {
        return new HarmonyMethod(AccessTools.Method(GetType(), name));
    }
}