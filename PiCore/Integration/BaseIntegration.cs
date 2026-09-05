/*
代码来源：Pathoschild
原始出处：https://github.com/Pathoschild/StardewMods
授权协议：MIT License
*/

using System;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;
using weizinai.StardewValleyMod.Common;

namespace weizinai.StardewValleyMod.PiCore.Integration;

/// <summary>The base implementation for a mod integration.</summary>
public abstract class BaseIntegration : IModIntegration
{
    /// <summary>A human-readable name for the mod.</summary>
    protected readonly string Label;

    /// <summary>The mod's unique ID.</summary>
    private readonly string modId;

    /// <summary>An API for fetching metadata about loaded mods.</summary>
    private readonly IModRegistry modRegistry;

    /// <inheritdoc />
    public virtual bool IsLoaded { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="label">A human-readable name for the mod.</param>
    /// <param name="modId">The mod's unique ID.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    protected BaseIntegration(string label, string modId, string minVersion, IModRegistry modRegistry)
    {
        // Init
        this.Label = label;
        this.modId = modId;
        this.modRegistry = modRegistry;

        // Validate mod
        var manifest = modRegistry.Get(modId)?.Manifest;

        if (manifest is null)
        {
            return;
        }

        if (manifest.Version.IsOlderThan(minVersion))
        {
            Logger.Warn($"Detected {label} {manifest.Version}, but need {minVersion} or later. Disabled integration with this mod.");
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
            Logger.Warn($"Detected {this.Label}, but couldn't fetch its API. Disabled integration with this mod.");
        }

        return api;
    }

    /// <summary>Assert that the integration is loaded.</summary>
    /// <exception cref="InvalidOperationException">The integration isn't loaded.</exception>
    protected virtual void AssertLoaded()
    {
        if (!this.IsLoaded)
        {
            throw new InvalidOperationException($"The {this.Label} integration isn't loaded.");
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

    /// <inheritdoc />
    protected BaseIntegration(string label, string modId, string minVersion, IModRegistry modRegistry)
        : base(label, modId, minVersion, modRegistry)
    {
        if (!this.IsLoaded)
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
            throw new InvalidOperationException($"The {this.Label} integration isn't loaded.");
        }
    }
}