/*
代码来源：Pathoschild
原始出处：https://github.com/Pathoschild/StardewMods
授权协议：MIT License
*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

/// <summary>Handles the logic for integrating with the Generic Mod Configuration Menu mod.</summary>
/// <typeparam name="TConfig">The mod configuration type.</typeparam>
public class GenericModConfigMenuIntegration<TConfig> : BaseIntegration<IGenericModConfigMenuApi> where TConfig : class, new()
{
    /// <summary>The manifest for the mod consuming the API.</summary>
    private readonly IManifest consumerManifest;

    /// <summary>Get the current config model.</summary>
    private readonly Func<TConfig> getConfig;

    /// <summary>Reset the config model to the default values.</summary>
    private readonly Action reset;

    /// <summary>Save and apply the current config model.</summary>
    private readonly Action save;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="monitor">An API for logging messages to the SMAPI console, attributed to the consuming mod.</param>
    /// <param name="manifest">The manifest for the mod consuming the API.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the mod's config to its default values.</param>
    /// <param name="save">Save the mod's current config to the <c>config.json</c> file.</param>
    public GenericModConfigMenuIntegration(IModRegistry modRegistry, IMonitor monitor, IManifest manifest, Func<TConfig> getConfig, Action reset, Action save)
        : base("Generic Mod Config Menu", "spacechase0.GenericModConfigMenu", "1.16.0", modRegistry, monitor)
    {
        this.consumerManifest = manifest;
        this.getConfig = getConfig;
        this.reset = reset;
        this.save = save;
    }

    /// <summary>Register a mod whose config can be edited through the UI.</summary>
    /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
    public GenericModConfigMenuIntegration<TConfig> Register(bool titleScreenOnly = false)
    {
        this.AssertLoaded();

        this.ModApi.Register(this.consumerManifest, this.reset, this.save, titleScreenOnly);

        return this;
    }

    /// <summary>Add a section title at the current position in the form.</summary>
    /// <param name="text">The title text shown in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the tooltip.</param>
    /// <param name="enable">Whether the option is enabled.</param>
    public GenericModConfigMenuIntegration<TConfig> AddSectionTitle(Func<string> text, Func<string>? tooltip = null, bool enable = true)
    {
        this.AssertLoaded();

        if (enable)
        {
            this.ModApi.AddSectionTitle(this.consumerManifest, text, tooltip);
        }

        return this;
    }

    /// <summary>Add a subheader at the current position in the form.</summary>
    /// <param name="text">The title text shown in the form.</param>
    /// <param name="enable">Whether the option is enabled.</param>
    public GenericModConfigMenuIntegration<TConfig> AddSubHeader(Func<string> text, bool enable = true)
    {
        this.AssertLoaded();

        if (enable)
        {
            this.ModApi.AddSubHeader(this.consumerManifest, text);
        }

        return this;
    }

    /// <summary>Add a paragraph of text at the current position in the form.</summary>
    /// <param name="text">The paragraph text to display.</param>
    /// <param name="enable">Whether the option is enabled.</param>
    public GenericModConfigMenuIntegration<TConfig> AddParagraph(Func<string> text, bool enable = true)
    {
        this.AssertLoaded();

        if (enable)
        {
            this.ModApi.AddParagraph(this.consumerManifest, text);
        }

        return this;
    }

    /// <summary>Add an image at the current position in the form.</summary>
    /// <param name="texture">The image texture to display.</param>
    /// <param name="texturePixelArea">The pixel area within the texture to display, or <c>null</c> to show the entire image.</param>
    /// <param name="scale">The zoom factor to apply to the image.</param>
    /// <param name="enable">Whether the option is enabled.</param>
    public GenericModConfigMenuIntegration<TConfig> AddImage(Func<Texture2D> texture, Rectangle? texturePixelArea = null, int scale = Game1.pixelZoom,
        bool enable = true)
    {
        this.AssertLoaded();

        if (enable)
        {
            this.ModApi.AddImage(this.consumerManifest, texture, texturePixelArea, scale);
        }

        return this;
    }

    /// <summary>Add a boolean option at the current position in the form.</summary>
    /// <param name="get"></param>
    /// <param name="set"></param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="enable">Whether the option is enabled.</param>
    /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged" />, or <c>null</c> to auto-generate a randomized ID.</param>
    public GenericModConfigMenuIntegration<TConfig> AddBoolOption(Func<TConfig, bool> get, Action<TConfig, bool> set, Func<string> name,
        Func<string>? tooltip = null, bool enable = true, string? fieldId = null)
    {
        this.AssertLoaded();

        if (enable)
        {
            this.ModApi.AddBoolOption(
                this.consumerManifest,
                () => get(this.getConfig()),
                value => set(this.getConfig(), value),
                name,
                tooltip,
                fieldId
            );
        }

        return this;
    }

    /// <summary>Add an integer option at the current position in the form.</summary>
    /// <param name="get"></param>
    /// <param name="set"></param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
    /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
    /// <param name="enable">Whether the option is enabled.</param>
    /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged" />, or <c>null</c> to auto-generate a randomized ID.</param>
    public GenericModConfigMenuIntegration<TConfig> AddNumberOption(Func<TConfig, int> get, Action<TConfig, int> set, Func<string> name,
        Func<string>? tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string>? formatValue = null, bool enable = true,
        string? fieldId = null)
    {
        this.AssertLoaded();

        if (enable)
        {
            this.ModApi.AddNumberOption(
                this.consumerManifest,
                () => get(this.getConfig()),
                value => set(this.getConfig(), value),
                name,
                tooltip,
                min,
                max,
                interval,
                formatValue,
                fieldId
            );
        }

        return this;
    }

    /// <summary>Add a float option at the current position in the form.</summary>
    /// <param name="get"></param>
    /// <param name="set"></param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
    /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
    /// <param name="enable">Whether the option is enabled.</param>
    /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged" />, or <c>null</c> to auto-generate a randomized ID.</param>
    public GenericModConfigMenuIntegration<TConfig> AddNumberOption(Func<TConfig, float> get, Action<TConfig, float> set, Func<string> name,
        Func<string>? tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string>? formatValue = null,
        bool enable = true, string? fieldId = null)
    {
        this.AssertLoaded();

        if (enable)
        {
            this.ModApi.AddNumberOption(
                this.consumerManifest,
                () => get(this.getConfig()),
                value => set(this.getConfig(), value),
                name,
                tooltip,
                min,
                max,
                interval,
                formatValue,
                fieldId
            );
        }

        return this;
    }

    /// <summary>Add a string option at the current position in the form.</summary>
    /// <param name="get"></param>
    /// <param name="set"></param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="allowedValues">The values that can be selected, or <c>null</c> to allow any.</param>
    /// <param name="formatAllowedValue">Get the display text to show for a value from <paramref name="allowedValues" />, or <c>null</c> to show the values as-is. </param>
    /// <param name="enable">Whether the option is enabled.</param>
    /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged" />, or <c>null</c> to auto-generate a randomized ID.</param>
    public GenericModConfigMenuIntegration<TConfig> AddTextOption(Func<TConfig, string> get, Action<TConfig, string> set, Func<string> name,
        Func<string>? tooltip = null, string[]? allowedValues = null, Func<string, string>? formatAllowedValue = null, bool enable = true,
        string? fieldId = null)
    {
        this.AssertLoaded();

        if (enable)
        {
            this.ModApi.AddTextOption(
                this.consumerManifest,
                () => get(this.getConfig()),
                value => set(this.getConfig(), value),
                name,
                tooltip,
                allowedValues,
                formatAllowedValue,
                fieldId
            );
        }

        return this;
    }

    /// <summary>Add a key binding at the current position in the form.</summary>
    /// <param name="get"></param>
    /// <param name="set"></param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="enable">Whether the option is enabled.</param>
    /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged" />, or <c>null</c> to auto-generate a randomized ID.</param>
    public GenericModConfigMenuIntegration<TConfig> AddKeybind(Func<TConfig, SButton> get, Action<TConfig, SButton> set, Func<string> name,
        Func<string>? tooltip = null, bool enable = true, string? fieldId = null)
    {
        this.AssertLoaded();

        if (enable)
        {
            this.ModApi.AddKeybind(
                this.consumerManifest,
                () => get(this.getConfig()),
                value => set(this.getConfig(), value),
                name,
                tooltip,
                fieldId
            );
        }

        return this;
    }

    /// <summary>Add a key binding list at the current position in the form.</summary>
    /// <param name="get"></param>
    /// <param name="set"></param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="enable">Whether the option is enabled.</param>
    /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged" />, or <c>null</c> to auto-generate a randomized ID.</param>
    public GenericModConfigMenuIntegration<TConfig> AddKeybindList(Func<TConfig, KeybindList> get, Action<TConfig, KeybindList> set, Func<string> name,
        Func<string>? tooltip = null, bool enable = true, string? fieldId = null)
    {
        this.AssertLoaded();

        if (enable)
        {
            this.ModApi.AddKeybindList(
                this.consumerManifest,
                () => get(this.getConfig()),
                value => set(this.getConfig(), value),
                name,
                tooltip,
                fieldId
            );
        }

        return this;
    }

    /// <summary>Start a new page in the mod's config UI, or switch to that page if it already exists. All options registered after this will be part of that page.</summary>
    /// <param name="pageId">The unique page ID.</param>
    /// <param name="pageTitle">The page title shown in its UI, or <c>null</c> to show the <paramref name="pageId" /> value.</param>
    /// <remarks>
    ///     You must also call <see cref="AddPageLink" /> to make the page accessible. This is only needed to set up a multi-page config UI. If you don't call
    ///     this method, all options will be part of the mod's main config UI instead.
    /// </remarks>
    public GenericModConfigMenuIntegration<TConfig> AddPage(string pageId, Func<string>? pageTitle = null)
    {
        this.AssertLoaded();

        this.ModApi.AddPage(this.consumerManifest, pageId, pageTitle);

        return this;
    }

    /// <summary>Add a link to a page added via <see cref="AddPage" /> at the current position in the form.</summary>
    /// <param name="pageId">The unique ID of the page to open when the link is clicked.</param>
    /// <param name="text">The link text shown in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the link, or <c>null</c> to disable the tooltip.</param>
    /// <param name="enable">Whether the option is enabled.</param>
    public GenericModConfigMenuIntegration<TConfig> AddPageLink(string pageId, Func<string> text, Func<string>? tooltip = null, bool enable = true)
    {
        this.AssertLoaded();

        if (enable)
        {
            this.ModApi.AddPageLink(this.consumerManifest, pageId, text, tooltip);
        }

        return this;
    }

    /// <summary>Add an option at the current position in the form using custom rendering logic.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="draw">Draw the option in the config UI. This is called with the sprite batch being rendered and the pixel position at which to start drawing.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="beforeMenuOpened">A callback raised just before the menu containing this option is opened.</param>
    /// <param name="beforeSave">A callback raised before the form's current values are saved to the config (i.e. before the <c>save</c> callback passed to <see cref="Register" />).</param>
    /// <param name="afterSave">A callback raised after the form's current values are saved to the config (i.e. after the <c>save</c> callback passed to <see cref="Register" />).</param>
    /// <param name="beforeReset">A callback raised before the form is reset to its default values (i.e. before the <c>reset</c> callback passed to <see cref="Register" />).</param>
    /// <param name="afterReset">A callback raised after the form is reset to its default values (i.e. after the <c>reset</c> callback passed to <see cref="Register" />).</param>
    /// <param name="beforeMenuClosed">A callback raised just before the menu containing this option is closed.</param>
    /// <param name="height">The pixel height to allocate for the option in the form, or <c>null</c> for a standard input-sized option.</param>
    /// <param name="enable">Whether the option is enabled.</param>
    /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged" />, or <c>null</c> to auto-generate a randomized ID.</param>
    public GenericModConfigMenuIntegration<TConfig> AddComplexOption(Func<string> name, Action<SpriteBatch, Vector2> draw,
        Func<string>? tooltip = null, Action? beforeMenuOpened = null, Action? beforeSave = null, Action? afterSave = null, Action? beforeReset = null,
        Action? afterReset = null, Action? beforeMenuClosed = null, Func<int>? height = null, bool enable = true, string? fieldId = null)
    {
        this.AssertLoaded();

        if (enable)
        {
            this.ModApi.AddComplexOption(
                this.consumerManifest,
                name,
                draw,
                tooltip,
                beforeMenuOpened,
                beforeSave,
                afterSave,
                beforeReset,
                afterReset,
                beforeMenuClosed,
                height,
                fieldId
            );
        }

        return this;
    }

    /// <summary>Set whether the options registered after this point can only be edited from the title screen.</summary>
    /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
    /// <remarks>This lets you have different values per-field. Most mods should just set it once in <see cref="Register" />.</remarks>
    public void SetTitleScreenOnlyForNextOptions(bool titleScreenOnly)
    {
        this.AssertLoaded();

        this.ModApi.SetTitleScreenOnlyForNextOptions(this.consumerManifest, titleScreenOnly);
    }

    /// <summary>Register a method to notify when any option registered by this mod is edited through the config UI.</summary>
    /// <param name="onChange">The method to call with the option's unique field ID and new value.</param>
    public void OnFieldChanged(Action<string, object> onChange)
    {
        this.AssertLoaded();

        this.ModApi.OnFieldChanged(this.consumerManifest, onChange);
    }

    /// <summary>Open the config UI for a specific mod.</summary>
    public void OpenModMenu()
    {
        this.AssertLoaded();

        this.ModApi.OpenModMenu(this.consumerManifest);
    }

    /// <summary>Open the config UI for a specific mod, as a child menu if there is an existing menu.</summary>
    public void OpenModMenuAsChildMenu()
    {
        this.AssertLoaded();

        this.ModApi.OpenModMenuAsChildMenu(this.consumerManifest);
    }

    /// <summary>Get the currently-displayed mod config menu, if any.</summary>
    /// <param name="mod">The manifest of the mod whose config menu is being shown, or <c>null</c> if not applicable.</param>
    /// <param name="page">The page ID being shown for the current config menu, or <c>null</c> if not applicable.</param>
    /// <returns>Returns whether a mod config menu is being shown.</returns>
    public bool TryGetCurrentMenu(out IManifest? mod, out string? page)
    {
        this.AssertLoaded();

        return this.ModApi.TryGetCurrentMenu(out mod, out page);
    }

    /// <summary>Remove a mod from the config UI and delete all its options and pages.</summary>
    public void Unregister()
    {
        this.AssertLoaded();

        this.ModApi.Unregister(this.consumerManifest);
    }
}