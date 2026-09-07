# PiCore 配置模块使用约定

本文是 PiCore 配置模块（`weizinai.StardewValleyMod.PiCore.Config`）的权威使用文档：约定成员形态规则、走一遍最短路线的接入步骤，并列出非显而易见的坑。源码 XML
摘要仍是逐方法细节的权威来源；本文只做约定与速查。

适用对象：在本仓库内编写「会序列化进模组 `config.json` 的配置类」或「通过 PiCore 接入 GMCM 配置菜单」的模组作者与 agent。

## ① 配置类成员形态规则

> **必须：** 每个序列化进模组 `config.json` 的类，其被序列化的成员一律是 **公共自动属性** `{ get; set; }`，需要默认值时写 **显式初始化器**； **不使用公共字段**
> 。整条规则的例外只有「不写初始化器 = 意图采用 CLR 默认值」，除此之外不承认任何例外（如"只读属性""内部 setter"都不属于配置类成员）。

适用对象不只是根 `ModConfig`：凡经根配置属性链引用、随根配置一起被 SMAPI 序列化的嵌套配置类（如 BetterCabin 的 `OnlineTimeConfig`、HelpWanted 的 `BaseQuestConfig`
）同样适用。纯内存模型类（不落盘、只做运行时数据）不在本规则约束内，可按需自定形态。

### 为什么是自动属性（理由）

- **模块契约只支持可写公共属性。** 配置模块的绑定与重置都以「公共可写属性」为成员契约：`ConfigMember.CreateAccessor` 用表达式树把成员访问编译成 get/set
  委托，成员链中任一环节是公共字段即拒绝（配置类不支持公共字段）；`ConfigService` 的重置逻辑（`CopyMemberValues`）只逐属性写回公共 setter
  属性。字段既不能进菜单绑定、也不会被重置还原，因此配置成员必须是公共自动属性。
- **JSON 契约相同。** SMAPI 读写 `config.json` 按 **成员名**匹配，键序与成员形态无关：同名公共字段与公共可写属性产出完全相同的键，加载按名取值、与文件里的键序无关。历史上字段形态的老配置文件照常按名加载——不会被重置、不会改键，无需玩家迁移。
- **可演进。** 自动属性将来可以在 setter/getter 里加校验或派生逻辑，而不改变 JSON 契约，也不改变调用方（`config.Foo` 对字段与属性写法一致）。

## ② 最短路线的端到端接线

以模组 `SomeMod` 为例。四步即可让「声明配置 → 读取（损坏自愈）→ 菜单渲染 → 保存/重置」全部跑通。

### 1) 声明配置类

`Framework/ModConfig.cs`：

```csharp
namespace weizinai.StardewValleyMod.SomeMod.Framework;

internal class ModConfig
{
    public static ModConfig Instance { get; set; } = null!;

    public bool SomeToggle { get; set; } = true;
    public KeybindList OpenMenuKey { get; set; } = new(SButton.F5);

    // 嵌套配置：自动属性 + new() 初始化器
    public SomeNestedConfig Nested { get; set; } = new();
}
```

要点：根 `ModConfig` 保留隐式无参构造（配置模块受 `class, new()` 约束）；被序列化成员一律是公共自动属性，需要默认值时写显式初始化器（无初始化器 = 采用 CLR 默认值）；
`Instance` 是静态单例入口，处理器（Handler）都从它读当前配置。

### 2) 在 `Entry` 交给 `ConfigService`

`ModEntry.cs`：

```csharp
public override void Entry(IModHelper helper)
{
    I18n.Init(helper.Translation);

    // 构造即读取 config.json；读取失败（损坏/玩家手写错误）会自动写回默认配置并重读，模组不会崩
    var configService = new ConfigService<ModConfig>(
        this,
        () => ModConfig.Instance,
        value => ModConfig.Instance = value,
        this.UpdateConfig   // 可选：保存或重置后重建处理器
    );
    configService.RegisterMenu(this.BuildConfigMenu);

    this.UpdateConfig();    // 按初始配置构建处理器（需在 Entry 里显式调用一次）
}
```

`ConfigService` 构造参数：消费模组入口 `IMod`、取当前配置的 `getConfig`、写入（初始读到的）配置的 `setConfig`、可选的 `onConfigChanged`
回调——保存与重置都会触发它一次，模组在其中重建/刷新依赖配置的处理器（本仓库各模组的 `UpdateConfig` 模式）。`onConfigChanged` 在「保存」与「重置」两个动作后统一触发，模组不需要分别挂两处逻辑。

### 3) 声明一次菜单

`ModEntry.cs`：

```csharp
/// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
{
    menu
        .AddSectionTitle(I18n.Config_GeneralSettingTitle_Name)
        .AddBoolOption(config => config.SomeToggle, I18n.Config_SomeToggle_Name)
        .AddKeybindListOption(config => config.OpenMenuKey, I18n.Config_OpenMenuKey_Name)
        .AddSection(
            config => config.Nested,
            I18n.Config_NestedTitle_Name,
            section => section
                .AddBoolOption(nested => nested.Enable, I18n.Config_Nested_Enable_Name)
        );
}
```

### 4) GMCM 渲染

`RegisterMenu` 只登记构建委托；真正的注册发生在 `GameLaunched` 触发时（若模组在 `GameLaunched` 之后才调用 `RegisterMenu` 则立即注册）。注册时执行构建委托、把描述器的动作逐个映射到
GMCM 集成对象。 **GMCM 未安装时是静默无操作**：配置的读取、损坏自愈、保存、重置照常工作，只是没有菜单可改——模组不能假设菜单存在。想用热键打开菜单时调用
`configService.OpenMenu()`。

## ③ 描述器成员绑定速查

`ConfigMenuDescriptor<TConfig>`（链式；标签/提示均传 `Func<string>`）：

| 你想渲染           | 调用                                                                                                                                                                                             |
|--------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 布尔开关           | `AddBoolOption(config => config.X, name)`                                                                                                                                                        |
| 布尔领头的分区     | `AddBoolSection(config => config.X, name)`（标签兼作分区标题）                                                                                                                                   |
| 整数数值           | `AddNumberOption(config => config.N, name, tooltip, min, max, interval)`                                                                                                                         |
| 浮点数值           | `AddNumberOption(config => config.F, name, tooltip, min, max, interval)`                                                                                                                         |
| 文本 / 下拉        | `AddTextOption(config => config.S, name, tooltip, allowedValues, formatAllowedValue)`                                                                                                            |
| 枚举（可本地化）   | `AddEnumOption<MyEnum>(config => config.E, name, tooltip, allowedValues, formatValue)`                                                                                                           |
| 按键绑定列表       | `AddKeybindListOption(config => config.Key, name)`                                                                                                                                               |
| 分区标题 / 段落    | `AddSectionTitle(name, tooltip)` / `AddParagraph(text)`                                                                                                                                          |
| 多页菜单           | `AddPage(pageId, title)` + `AddPageLink(pageId, linkText)`                                                                                                                                       |
| 嵌套配置作分区     | `AddSection(config => config.Nested, title, section => …, tooltip)`，`section` 是 `ConfigMenuSection<TConfig, TSection>`，其选项方法与上面一一对应（按子配置成员绑定，渲染时自动拼回根配置读写） |
| 逃生舱（长尾场景） | `AddCustomSection(rawMenu => …)`，直接拿 `GenericModConfigMenuIntegration<TConfig>` 调原始 GMCM API                                                                                              |

## ④ 坑

- **绑定目标必须是公共可写属性。** get-only 属性、常量以及 **公共字段**会在 `ConfigMember.CreateAccessor` 抛
  `ArgumentException`（"配置选项成员必须是一段可赋值的公共属性链…"／"配置成员必须是公共自动属性，不支持公共字段…"）；非公共 setter 的属性能绑上但会
  **绕过可见性写入**（表达式编译的委托在全信任下可直接调私有 setter），静默违背 ① 规则——所以绑定成员与嵌套配置都必须是带公共 setter 的自动属性。
- **重置只复制「公共 setter 属性」。** `ConfigService.Reset` 的 `CopyMemberValues` 逐属性遍历，仅当属性 `SetMethod` 是公共的才写回；非公共 setter
  的属性在重置后保持原值。公共字段不在复制范围——配置模块的成员契约仅限公共可写属性。
- **重置会**按引用 **替换嵌套配置对象。** 重置把当前根实例的嵌套属性重新赋值为 `new TConfig()`
  里新建的默认子对象（根实例引用保持不变，嵌套子对象的引用被替换）。因此在重置前捕获了嵌套子对象引用的代码会读到旧对象——应通过根配置（`ModConfig.Instance`）读取，或在
  `onConfigChanged` 里重建依赖。
- **标签/提示是 `Func<string>`。** 来自 ModTranslationClassBuilder 生成的强类型 `I18n` 访问器（键漏写会在编译期报错）。渲染期每次取值，故语言切换即时生效。需要具体字符串时调用它（
  `I18n.Config_X_Name()`），需要传给选项方法时直接传访问器。
- **枚举渲染按成员名走。** `AddEnumOption` 以文本选项呈现，值用成员名映射、可经 `formatValue` 本地化、可经 `allowedValues` 决定可选顺序与子集。枚举成员本身仍是强类型枚举，不落成字符串键。
- **字典成员不能走成员绑定。** 字典下标读写编成 `get_Item`/`set_Item` 方法调用，不是可赋值的公共属性链，`ConfigMember.CreateAccessor` 不接受。要渲染字典（如 LazyMod
  按成长阶段存开关的 `Dictionary<int,bool>`）必须用 `AddCustomSection` 逃生舱按原始 GMCM API 逐个注册（LazyMod 的 `AddTreeSettingsPage` 即此用法）。
- **`KeybindList` 用专用选项。** 按键绑定列表只能经 `AddKeybindListOption`（底层 `AddKeybindList`），不要试图按文本/枚举处理。
