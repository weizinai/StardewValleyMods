using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Config;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Helper;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Patcher;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.UI;
using weizinai.StardewValleyMod.PiCore.Config;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger<ModEntry>.Init(this);
        // 配置模块接管读取（损坏自愈）、GMCM 生命周期与保存/重置，读到的实例写入静态 ModConfig.Instance 供菜单与事件处理器读取
        var configService = new ConfigService<ModConfig>(
            this,
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
        configService.RegisterMenu(this.BuildConfigMenu);
        OptionCatalog.Init(helper);
        TextureManager.Instance.LoadTexture(helper);
        // 注册事件
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new Game1Patcher());
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        var config = ModConfig.Instance;

        if (config.OpenMenuByTelephone) return;

        if (config.MenuKey.JustPressed())
        {
            if (MenuLauncher.IsOpen())
                Game1.exitActiveMenu();
            else if (Context.IsPlayerFree) MenuLauncher.Open(config.DefaultMenuTabId);
        }
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            .AddKeybindListOption(config => config.MenuKey, I18n.Config_MenuKey)
            .AddBoolOption(
                config => config.OpenMenuByTelephone,
                I18n.Config_OpenMenuByTelephone,
                I18n.Config_OpenMenuByTelephone_Tooltip
            )
            .AddTextOption(
                config => config.DefaultMenuTabId,
                I18n.Config_DefaultMenuTab,
                allowedValues: OptionCatalog.GetAllTabIds,
                formatAllowedValue: OptionCatalog.GetTabTitle
            )
            .AddBoolOption(config => config.ProgressMode, I18n.Config_ProgressMode)
            .AddKeybindListOption(config => config.FavoriteKey, I18n.Config_FavoriteKey);
    }
}
