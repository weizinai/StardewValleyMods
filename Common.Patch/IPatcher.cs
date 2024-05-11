using HarmonyLib;

namespace Common.Patch;

internal interface IPatcher
{
    public void Apply(Harmony harmony);
}