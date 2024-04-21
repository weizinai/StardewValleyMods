using HarmonyLib;
using StardewModdingAPI;

namespace Common.Patch;

public interface IPatcher
{
    public void Patch(Harmony harmony);
}