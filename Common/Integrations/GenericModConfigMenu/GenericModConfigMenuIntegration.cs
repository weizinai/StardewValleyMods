using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace Common.Integrations;

/// <summary>Handles the logic for integrating with the Generic Mod Config Menu mod.</summary>
/// <typeparam name="TConfig">The mod config type.</typeparam>
internal class GenericModConfigMenuIntegration<TConfig> where TConfig : new()
{
    /// <summary>The manifest for the mod consuming the API.</summary>
    private readonly IManifest consumerManifest;

    /// <summary>Get the current config model.</summary>
    private readonly Func<TConfig> getConfig;

    /// <summary>Reset the mod's config to its default values.</summary>
    private readonly Action reset;

    /// <summary>Save the mod's current config to the <c>config.json</c> file.</summary>
    private readonly Action save;

    /// <summary>The mod's public API.</summary>
    private readonly IGenericModConfigMenuAPI? configMenu;

    /// <summary>Whether the mod is available.</summary>
    public bool IsLoaded => configMenu != null;


    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="consumerManifest">The manifest for the mod consuming the API.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the mod's config to its default values.</param>
    /// <param name="save">Save the mod's current config to the <c>config.json</c> file.</param>
    public GenericModConfigMenuIntegration(IModRegistry modRegistry, IManifest consumerManifest, Func<TConfig> getConfig, Action reset, Action save)
    {
        this.consumerManifest = consumerManifest;
        this.getConfig = getConfig;
        this.reset = reset;
        this.save = save;
        configMenu = modRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
    }

    /// <summary>Register a mod whose config can be edited through the UI.</summary>
    /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
    public GenericModConfigMenuIntegration<TConfig> Register(bool titleScreenOnly = false)
    {
        configMenu?.Register(consumerManifest, reset, save, titleScreenOnly);
        return this;
    }

    /// <summary>Add a section title at the current position in the form.</summary>
    /// <param name="text">The title text shown in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the tooltip.</param>
    public GenericModConfigMenuIntegration<TConfig> AddSectionTitle(Func<string> text, Func<string>? tooltip = null)
    {
        configMenu?.AddSectionTitle(consumerManifest, text, tooltip);
        return this;
    }

    /// <summary>Add a paragraph of text at the current position in the form.</summary>
    /// <param name="text">The paragraph text to display.</param>
    public GenericModConfigMenuIntegration<TConfig> AddParagraph(Func<string> text)
    {
        configMenu?.AddParagraph(consumerManifest, text);
        return this;
    }

    /// <summary>Add a boolean option at the current position in the form.</summary>
    /// <param name="get"></param>
    /// <param name="set"></param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    public GenericModConfigMenuIntegration<TConfig> AddBoolOption(Func<TConfig, bool> get, Action<TConfig, bool> set, Func<string> name, Func<string>? tooltip = null)
    {
        configMenu?.AddBoolOption(
            consumerManifest,
            () => get(getConfig()),
            value => set(getConfig(), value),
            name,
            tooltip
        );
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
    public GenericModConfigMenuIntegration<TConfig> AddNumberOption(Func<TConfig, int> get, Action<TConfig, int> set, Func<string> name,
        Func<string>? tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string>? formatValue = null)
    {
        configMenu?.AddNumberOption(
            consumerManifest,
            () => get(getConfig()),
            value => set(getConfig(), value),
            name,
            tooltip,
            min,
            max,
            interval,
            formatValue
        );
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
    public GenericModConfigMenuIntegration<TConfig> AddNumberOption(Func<TConfig, float> get, Action<TConfig, float> set, Func<string> name,
        Func<string>? tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string>? formatValue = null)
    {
        configMenu?.AddNumberOption(
            consumerManifest,
            () => get(getConfig()),
            value => set(getConfig(), value),
            name,
            tooltip,
            min,
            max,
            interval,
            formatValue
        );
        return this;
    }

    /// <summary>Add a string option at the current position in the form.</summary>
    /// <param name="get"></param>
    /// <param name="set"></param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="allowedValues">The values that can be selected, or <c>null</c> to allow any.</param>
    /// <param name="formatAllowedValue">Get the display text to show for a value from <paramref name="allowedValues" />, or <c>null</c> to show the values as-is. </param>
    public GenericModConfigMenuIntegration<TConfig> AddTextOption(Func<TConfig, string> get, Action<TConfig, string> set, Func<string> name, Func<string>? tooltip = null,
        string[]? allowedValues = null, Func<string, string>? formatAllowedValue = null)
    {
        configMenu?.AddTextOption(
            consumerManifest,
            () => get(getConfig()),
            value => set(getConfig(), value),
            name,
            tooltip,
            allowedValues,
            formatAllowedValue
        );
        return this;
    }

    /// <summary>Add a key binding list at the current position in the form.</summary>
    /// <param name="get"></param>
    /// <param name="set"></param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    public GenericModConfigMenuIntegration<TConfig> AddKeybindList(Func<TConfig, KeybindList> get, Action<TConfig, KeybindList> set, Func<string> name, Func<string>? tooltip = null)
    {
        configMenu?.AddKeybindList(
            consumerManifest,
            () => get(getConfig()),
            value => set(getConfig(), value),
            name,
            tooltip
        );
        return this;
    }

    /// <summary>Start a new page in the mod's config UI, or switch to that page if it already exists. All options registered after this will be part of that page.</summary>
    /// <param name="pageId">The unique page ID.</param>
    /// <param name="pageTitle">The page title shown in its UI, or <c>null</c> to show the <paramref name="pageId" /> value.</param>
    /// <remarks>
    ///     You must also call <see cref="AddPageLink" /> to make the page accessible. This is only needed to set up a multi-page config UI. If you don't call this method,
    ///     all options will be part of the mod's main config UI instead.
    /// </remarks>
    public GenericModConfigMenuIntegration<TConfig> AddPage(string pageId, Func<string>? pageTitle = null)
    {
        configMenu?.AddPage(consumerManifest, pageId, pageTitle);
        return this;
    }

    /// <summary>Add a link to a page added via <see cref="AddPage" /> at the current position in the form.</summary>
    /// <param name="pageId">The unique ID of the page to open when the link is clicked.</param>
    /// <param name="text">The link text shown in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the link, or <c>null</c> to disable the tooltip.</param>
    public GenericModConfigMenuIntegration<TConfig> AddPageLink(string pageId, Func<string> text, Func<string>? tooltip = null)
    {
        configMenu?.AddPageLink(consumerManifest, pageId, text, tooltip);
        return this;
    }

    /// <summary>Open the config UI for a specific mod.</summary>
    public void OpenMenu()
    {
        configMenu?.OpenModMenu(consumerManifest);
    }
}