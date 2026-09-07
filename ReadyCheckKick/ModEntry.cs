using StardewModdingAPI;
using weizinai.StardewValleyMod.PiCore.Config;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.ReadyCheckKick.Framework;
using weizinai.StardewValleyMod.ReadyCheckKick.Handler;
using weizinai.StardewValleyMod.ReadyCheckKick.Patcher;

namespace weizinai.StardewValleyMod.ReadyCheckKick;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(this.Helper.Translation);
        Logger<ModEntry>.Init(this);
        // 配置模块接管读取（损坏自愈）、GMCM 生命周期与保存/重置，读到的实例写入静态 ModConfig.Instance 供事件处理器读取
        var configService = new ConfigService<ModConfig>(
            this,
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
        configService.RegisterMenu(this.BuildConfigMenu);
        // 注册Harmony补丁与事件处理器（处理器在 Apply 内部订阅事件）
        HarmonyPatcher.Apply(this, new SaveGameMenuPatcher(helper.Reflection));
        new ReadyCheckDialogueHandler(helper).Apply();
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            // Show unready farmers
            .AddSectionTitle(I18n.Config_ShowUnreadyFarmersTitle_Name)
            .AddBoolOption(config => config.ShowInfoInReadyCheckDialogue, I18n.Config_ShowInfoInReadyCheckDialogue_Name)
            .AddBoolOption(config => config.ShowInfoInSaveGameMenu, I18n.Config_ShowInfoInSaveGameMenu_Name)
            // Kick unready farmers
            .AddSectionTitle(I18n.Config_KickUnreadyFarmersTitle_Name)
            .AddBoolOption(config => config.AutoKickUnreadyFarmers, I18n.Config_AutoKickUnreadyFarmers_Name)
            .AddNumberOption(
                config => config.AutoKickUnreadyFarmersRatio,
                I18n.Config_AutoKickUnreadyFarmersRatio_Name,
                min: 0f,
                max: 1f,
                interval: 0.05f
            )
            .AddNumberOption(config => config.AutoKickUnreadyFarmersDelay, I18n.Config_AutoKickUnreadyFarmersDelay_Name);
    }
}
