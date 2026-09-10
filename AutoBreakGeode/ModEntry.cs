using StardewModdingAPI;
using weizinai.StardewValleyMod.AutoBreakGeode.Config;
using weizinai.StardewValleyMod.AutoBreakGeode.Handler;
using weizinai.StardewValleyMod.AutoBreakGeode.Speed;
using weizinai.StardewValleyMod.PiCore.Config;

namespace weizinai.StardewValleyMod.AutoBreakGeode;

internal class ModEntry : Mod
{
    private AutoBreakHandler autoBreakHandler = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        // 速度策略在装配期建好：它探明的「动画速度归谁」既是配置菜单的适用性判据、也是处理器补帧的前提，
        // 两个消费者共用这一个实例，菜单由此不必反向读运行时处理器
        var speedPolicy = new GeodeSpeedPolicy(helper);
        // 配置模块接管读取（损坏自愈）、GMCM 生命周期与保存/重置，读到的实例写入静态 ModConfig.Instance 供各处现读
        // （处理器读快捷键，速度策略读倍率）
        // 这里只登记构建委托，菜单构建延后到 GameLaunched（见 PiCore 的 ConfigService.RegisterMenu）
        new ConfigService<ModConfig>(
            this,
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        ).RegisterMenu(menu => this.BuildConfigMenu(menu, speedPolicy));
        // 注册事件（本模组不打任何 Harmony 补丁，交互全部走 SMAPI 事件）
        // 只 Apply 一次、不进配置变更重建流程：理由见 AutoBreakHandler 的类注释
        this.autoBreakHandler = new AutoBreakHandler(helper, speedPolicy);
        this.autoBreakHandler.Apply();
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    /// <param name="speedPolicy">动画速度归属策略，与处理器共用同一个实例。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu, GeodeSpeedPolicy speedPolicy)
    {
        // 速度上限照抄 Fast Animations 自己的界面范围（1..20，见其 Framework/ModConfig.cs 与 GMCM 集成）：
        // min 与 max 同时给出才会渲染成有边界的滑条，否则 GMCM 退化成无边界文本框，玩家能填 0、负数或 100000
        // （0 与负数等于关掉补帧，巨大值等于每帧巨量重调 update），且原样写进 config.json。
        // 装了 Fast Animations 时本模组的补帧整个不生效（原因见 GeodeSpeedPolicy.IsExternallyControlled），故索性不注册这个选项：
        // enable 在注册菜单时求值，即 GameLaunched 之后，那时策略已探明「装了没装」。
        // 上限与「装了没装」都从策略取：它们是同一条判断的两处用法，留一处定义免得滑条与循环边界漂移
        menu
            .AddKeybindListOption(config => config.ToggleAutoBreakKeybind, I18n.Config_ToggleAutoBreakKeybind)
            .AddNumberOption(
                config => config.BreakGeodeSpeed,
                I18n.Config_GeodeAnimationSpeed,
                min: 1, max: GeodeSpeedPolicy.MaxSpeed, interval: 1,
                enable: !speedPolicy.IsExternallyControlled
            );
    }
}
