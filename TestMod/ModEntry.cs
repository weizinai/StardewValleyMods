using Common.Integration;
using Common.Patch;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TestMod.Framework;
using TestMod.Framework.Hud;
using TestMod.Patches;

namespace TestMod;

public class ModEntry : Mod
{
    private ModConfig config = null!;
    private MiningHud miningHud = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        miningHud = new MiningHud(config);
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Display.RenderedHud += OnRenderedHud;
        // 注册Harmony补丁
        HarmonyPatcher.Patch(this, new MineShaftPatcher(config), new VolcanoDungeonPatcher(config));
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        miningHud.Update();   
    }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        miningHud.Draw(e.SpriteBatch);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

        if (configMenu is null) return;

        configMenu.Register(
            ModManifest,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.MineShaftMap,
            value => config.MineShaftMap = value,
            I18n.Config_MineShaftMap_Name,
            I18n.Config_MineShaftMap_ToolTip,
            40,
            60
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => config.VolcanoDungeonMap,
            value => config.VolcanoDungeonMap = value,
            I18n.Config_VolcanoDungeonMap_Name,
            I18n.Config_VolcanoDungeonMap_ToolTip,
            38,
            57
        );
        
        // 显示矿井信息
        configMenu.AddSectionTitle(
            ModManifest,
            I18n.Config_ShowMineShaftInfo_Name
        );
        // 显示梯子信息
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowLadderInfo,
            value => config.ShowLadderInfo = value,
            I18n.Config_ShowLadderInfo_Name
        );
        // 显示竖井信息
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowShaftInfo,
            value => config.ShowShaftInfo = value,
            I18n.Config_ShowShaftInfo_Name
        );
        // 显示怪物信息
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowMonsterInfo,
            value => config.ShowMonsterInfo = value,
            I18n.Config_ShowMonsterInfo_Name
        );
        // 显示矿物信息
        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowMineralInfo,
            value => config.ShowMineralInfo = value,
            I18n.Config_ShowMineralInfo_Name
        );
    }
}