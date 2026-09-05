/*
代码来源：Pathoschild
原始出处：https://github.com/Pathoschild/StardewMods
授权协议：MIT License
*/

namespace weizinai.StardewValleyMod.PiCore.Integration;

/// <summary>Handles integration with a given mod.</summary>
internal interface IModIntegration
{
    /// <summary>Whether the mod is available.</summary>
    public bool IsLoaded { get; }
}