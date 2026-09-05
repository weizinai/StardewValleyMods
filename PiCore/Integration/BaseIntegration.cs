/*
代码来源：Pathoschild
原始出处：https://github.com/Pathoschild/StardewMods
授权协议：MIT License
*/

using System;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;

namespace weizinai.StardewValleyMod.PiCore.Integration;

/// <summary>The base implementation for a mod integration.</summary>
public abstract class BaseIntegration : IModIntegration
{
    /// <summary>A human-readable name for the mod.</summary>
    protected readonly string label;

    /// <summary>The mod's unique ID.</summary>
    private readonly string modId;

    /// <summary>An API for fetching metadata about loaded mods.</summary>
    private readonly IModRegistry modRegistry;

    /// <summary>An API for logging messages to the SMAPI console, attributed to the consuming mod.</summary>
    private readonly IMonitor monitor;

    /// <inheritdoc />
    public virtual bool IsLoaded { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="label">A human-readable name for the mod.</param>
    /// <param name="modId">The mod's unique ID.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="monitor">An API for logging messages to the SMAPI console.</param>
    protected BaseIntegration(string label, string modId, string minVersion, IModRegistry modRegistry, IMonitor monitor)
    {
        // Init
        this.label = label;
        this.modId = modId;
        this.modRegistry = modRegistry;
        this.monitor = monitor;

        // Validate mod
        var manifest = modRegistry.Get(modId)?.Manifest;

        if (manifest is null)
        {
            return;
        }

        if (manifest.Version.IsOlderThan(minVersion))
        {
            this.monitor.Log($"Detected {label} {manifest.Version}, but need {minVersion} or later. Disabled integration with this mod.", LogLevel.Warn);

            return;
        }

        this.IsLoaded = true;
    }

    /// <summary>Get an API for the mod, and show a message if it can't be loaded.</summary>
    /// <typeparam name="TApi">The API type.</typeparam>
    protected TApi? GetValidateApi<TApi>() where TApi : class
    {
        var api = this.modRegistry.GetApi<TApi>(this.modId);

        if (api is null)
        {
            this.monitor.Log($"Detected {this.label}, but couldn't fetch its API. Disabled integration with this mod.", LogLevel.Warn);
        }

        return api;
    }

    /// <summary>Assert that the integration is loaded.</summary>
    /// <exception cref="InvalidOperationException">The integration isn't loaded.</exception>
    protected virtual void AssertLoaded()
    {
        if (!this.IsLoaded)
        {
            throw new InvalidOperationException($"The {this.label} integration isn't loaded.");
        }
    }
}

/// <summary>The base implementation for a mod integration.</summary>
/// <typeparam name="TApi">The API type.</typeparam>
public class BaseIntegration<TApi> : BaseIntegration where TApi : class
{
    /// <summary>The mod's public API.</summary>
    public TApi? ModApi { get; }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(ModApi))]
    public override bool IsLoaded => this.ModApi is not null;

    /// <summary>Construct an instance.</summary>
    /// <param name="label">A human-readable name for the mod.</param>
    /// <param name="modId">The mod's unique ID.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="monitor">An API for logging messages to the SMAPI console.</param>
    protected BaseIntegration(string label, string modId, string minVersion, IModRegistry modRegistry, IMonitor monitor)
        : base(label, modId, minVersion, modRegistry, monitor)
    {
        // 只在基类判定加载（minVersion 门槛通过）后才拉取 API，避免绕过版本门槛
        if (base.IsLoaded)
        {
            this.ModApi = this.GetValidateApi<TApi>();
        }
    }

    /// <inheritdoc />
    [MemberNotNull(nameof(ModApi))]
    protected override void AssertLoaded()
    {
        if (!this.IsLoaded)
        {
            throw new InvalidOperationException($"The {this.label} integration isn't loaded.");
        }
    }
}
