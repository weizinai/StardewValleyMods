using weizinai.StardewValleyMod.Common.Integration;

namespace TestMod.Framework;

internal class GenericModConfigMenuIntegrationForTestMod
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForTestMod(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        this.configMenu = configMenu;
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            .AddBoolOption(
                config => config.EnableChangeCardChance,
                (config, value) => config.EnableChangeCardChance = value,
                () => "启用卡片概率修改"
            )
            .AddNumberOption(
                config => config.CardChance,
                (config, value) => config.CardChance = value,
                () => "卡片概率"
            );
    }
}