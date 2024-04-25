using HarmonyLib;

namespace Common.Patch;

public interface IPatcher
{
    public void Patch(Harmony harmony);
}