# PiCore 配置框架使用约定

> 面向**框架使用者**（模组作者与 AI 协作 agent）：本文档自包含，教你用 `PiCore.Config` 把「声明配置 → 读取 → 菜单渲染 → 保存/重置」全部跑通。API 签名以代码为准，本文档是对框架的导航与使用约定；两者不一致时以代码为准并请修正本文档。
>
> 适用版本：PiCore **0.5.0**（待定，未发布）。命名空间 `weizinai.StardewValleyMod.PiCore.Config`（下文简称 `PiCore.Config`）。同目录另见 [UI 框架使用说明](ui-framework.md)。

## 验证状态说明

「已验证」= 本仓库十余个模组实战使用过，路径经过真实玩家与真实模组检验。与 UI 框架不同，配置框架的四个公开类型全部有真实消费方，**无「未验证」类型**，无需回退提示。

| 状态 | 类型 |
| --- | --- |
| ✅ 已验证 | `ConfigService<TConfig>` · `ConfigMenuDescriptor<TConfig>` · `ConfigMenuSection<TConfig, TSection>` · `ConfigMember`（internal） |

实战消费模组：`AutoBreakGeode` · `LazyMod` · `BetterCabin` · `HelpWanted` · `ReadyCheckKick` · `FreeLock` · `FastControlInput` · `FriendshipDecayModify` · `ActiveMenuAnywhere` · `SpectatorMode` · `MultiplayerModLimit` · `CustomMineRefresh` · `SomeMultiplayerFeature` · `TestMod`。

---

# 1. 概述

## 1.1 框架定位

PiCore 配置框架是一条**声明式配置管线**：你只写一个「配置类 + 一段菜单声明」，读取（含损坏自愈）、GMCM 注册、保存、重置与回调都由框架统一承担。它取代了每模组手写 `ReadConfig` + `WriteConfig` + GMCM 事件里逐个注册的做法。

- **声明式**：配置类声明成员形态，菜单用链式 `Add*` 描述，框架把描述映射到 GMCM。
- **统一生命周期**：读取、注册、保存、重置收敛到 `ConfigService` 一处，回调统一走 `onConfigChanged`。
- **损坏自愈**：`config.json` 解析失败自动写回默认配置并重读，模组不会崩。

## 1.2 组成地图

| 类型 | 命名空间 | 职责 |
| --- | --- | --- |
| `ConfigService<TConfig>` | `...Config` | 生命周期：读取（自愈）/ 注册 / 保存 / 重置 / 回调 |
| `ConfigMenuDescriptor<TConfig>` | `...Config` | 声明式菜单描述器（链式 `Add*`） |
| `ConfigMenuSection<TConfig, TSection>` | `...Config` | 嵌套子配置作分区的构建器 |
| `ConfigMember` | `...Config`（internal） | 表达式树把成员访问编译成 get/set 委托，或拼接嵌套成员路径 |

## 1.3 生命周期

```
1. 声明配置类   —— 公共自动属性 + 显式初始化器（见 #3）
2. 交给 ConfigService —— Entry 里构造：立即读取 config.json（损坏自愈），写入模组持有的位置
3. 登记菜单     —— RegisterMenu(buildMenu) 只保存构建委托
4. 注册(启动)   —— GameLaunched 触发时执行构建委托，映射到 GMCM；未装 GMCM 则静默跳过
5. 保存/重置    —— 玩家在菜单操作或代码调用 Save()/Reset()，各触发 onConfigChanged 一次
6. 变更回调     —— 模组在 onConfigChanged 里重建/刷新依赖配置的处理器
```

**核心契约：所有被序列化成员必须是带公共 setter 的自动属性**（见 #3），绑定与重置都以此为准。

## 1.4 术语对照

| 中文 | 英文 | 含义 |
| --- | --- | --- |
| 配置类 | `TConfig` / `ModConfig` | 序列化进 `config.json` 的类，`class, new()` |
| 成员 | `member` | 配置类上的公共自动属性 |
| 绑定 | binding | 把成员访问表达式编译成读写委托 |
| 描述器 | `ConfigMenuDescriptor` | 声明菜单的链式对象 |
| 分区 | `ConfigMenuSection` | 嵌套子配置作菜单分区的构建器 |
| 逃生舱 | `AddCustomSection` | 拿到原始 GMCM 集成对象自行添加复杂选项 |
| 自愈 | self-heal | 配置读取失败时自动重置为默认 |

---

# 2. 快速上手（教程）

> 本章为 ✅ 已验证路径（以本仓库模组的真实用法为蓝本，重写为最小自包含示例）。跟着走完能得到一个完整可读/写/重制的配置 + GMCM 菜单。

以模组 `SomeMod` 为例。四步即可让「声明配置 → 读取（损坏自愈）→ 菜单渲染 → 保存/重置」全部跑通。

## 2.1 声明配置类

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

要点：根 `ModConfig` 保留隐式无参构造（框架受 `class, new()` 约束）；被序列化成员一律是公共自动属性，需要默认值时写显式初始化器（无初始化器 = 采用 CLR 默认值）；`Instance` 是静态单例入口，处理器（Handler）都从它读当前配置。

## 2.2 在 `Entry` 交给 `ConfigService`

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

`ConfigService` 构造参数：消费模组入口 `IMod`、取当前配置的 `getConfig`、写入（初始读到的）配置的 `setConfig`、可选的 `onConfigChanged` 回调——保存与重置都会触发它一次，模组在其中重建/刷新依赖配置的处理器（本仓库各模组的 `UpdateConfig` 模式）。`onConfigChanged` 在「保存」与「重置」两个动作后统一触发，模组不需要分别挂两处逻辑。

## 2.3 声明一次菜单

`ModEntry.cs`：

```csharp
/// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由框架渲染。</summary>
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

## 2.4 注册与生命周期

`RegisterMenu(buildMenu, titleScreenOnly: false)` 只登记构建委托；真正的注册发生在 `GameLaunched` 触发时（若模组在 `GameLaunched` 之后才调用 `RegisterMenu` 则立即注册）。注册时执行构建委托、把描述器的动作逐个映射到 GMCM 集成对象。`titleScreenOnly: true` 时菜单仅在标题界面可编辑（如联机配置）。 **GMCM 未安装时是静默无操作**：配置的读取、损坏自愈、保存、重置照常工作，只是没有菜单可改——模组不能假设菜单存在。

菜单打开方式：
- `configService.OpenMenu()` —— 立即打开本模组菜单（供热键驱动）。
- `configService.ReloadMenu()` —— 卸载并重新注册菜单。控制台命令改完动态配置（如可用 `allowedValues` 的列表键）后调用，会重跑菜单构建委托以刷新下拉选项。

---

# 3. 配置类成员形态规则

> 本章为 ✅ 已验证（本仓库所有带配置的模组共用同一规则）。

## 3.1 成员形态约定

> **必须：** 每个序列化进模组 `config.json` 的类，其被序列化的成员一律是 **公共自动属性** `{ get; set; }`，需要默认值时写 **显式初始化器**； **不使用公共字段**。整条规则的例外只有「不写初始化器 = 意图采用 CLR 默认值」，除此之外不承认任何例外（如「只读属性」「内部 setter」都不属于配置类成员）。

适用对象不只是根 `ModConfig`：凡经根配置属性链引用、随根配置一起被 SMAPI 序列化的嵌套配置类（如 BetterCabin 的 `OnlineTimeConfig`、HelpWanted 的 `BaseQuestConfig`）同样适用。纯内存模型类（不落盘、只做运行时数据）不在本规则约束内，可按需自定形态。

## 3.2 为什么是自动属性

- **模块契约只支持可写公共属性。** 配置结构的绑定与重置都以「公共可写属性」为成员契约：`ConfigMember.CreateAccessor` 用表达式树把成员访问编译成 get/set 委托，成员链中任一环节是公共字段即拒绝（配置类不支持公共字段）；`ConfigService` 的重置逻辑（`CopyMemberValues`）只逐属性写回公共 setter 属性。字段既不能进菜单绑定、也不会被重置还原，因此配置成员必须是公共自动属性。
- **JSON 契约相同。** SMAPI 读写 `config.json` 按 **成员名**匹配，键序与成员形态无关：同名公共字段与公共可写属性产出完全相同的键，加载按名取值、与文件里的键序无关。历史上字段形态的老配置文件照常按名加载——不会被重置、不会改键，无需玩家迁移。
- **可演进。** 自动属性将来可以在 setter/getter 里加校验或派生逻辑，而不改变 JSON 契约，也不改变调用方（`config.Foo` 对字段与属性写法一致）。

---

# 4. 描述器成员绑定速查

> 本章为 ✅ 已验证（每个选项方法都有真实调用方）。

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
| 嵌套配置作分区     | `AddSection(config => config.Nested, title, section => …, tooltip)`，`section` 是 `ConfigMenuSection<TConfig, TSection>`，其选项方法与上面一一对应（按子配置成员绑定，渲染中自动拼回根配置读写） |
| 逃生舱（长尾场景） | `AddCustomSection(rawMenu => …)`，直接拿 `GenericModConfigMenuIntegration<TConfig>` 调原始 GMCM API                                                                                              |

> 所有 `Add*` 选项方法都带可选的 `enable` 参数（默认 true）：传 `false` 时该项不渲染——适合按运行时条件显隐菜单元素（如低配模组未安装时隐藏相关选项）。枚举、数值等 `formatValue`/`formatAllowedValue` 为 null 时按默认格式显示。

---

# 5. 坑

> 本章教训 ✅ 已验证（均为真实踩过并修复的坑）。

- **绑定目标必须是公共可写属性。** 公共**字段**（含常量）会在 `ConfigMember.CreateAccessor` 抛
  `ArgumentException`（「配置选项成员必须是一段可赋值的公共属性链…」／「配置成员必须是公共自动属性，不支持公共字段…」）。get-only 或带非公共 setter 的属性能绑上，但会
  **绕过可见性写入**（表达式编译的委托在全信任下可直接调私有 setter）且不会被 `Reset` 复制——静默违背 #3 的成员形态规则。所以绑定成员与嵌套配置都必须是带公共 setter 的自动属性。
- **重置只复制「公共 setter 属性」。** `ConfigService.Reset` 的 `CopyMemberValues` 逐属性遍历，仅当属性 `SetMethod` 是公共的才写回；非公共 setter 的属性在重置后保持原值。公共字段不在复制范围——框架的成员契约仅限公共可写属性。
- **重置会**按引用 **替换嵌套配置对象。** 重置把当前根实例的嵌套属性重新赋值为 `new TConfig()` 里新建的默认子对象（根实例引用保持不变，嵌套子对象的引用被替换）。因此在重置前捕获了嵌套子对象引用的代码会读到旧对象——应通过根配置（`ModConfig.Instance`）读取，或在 `onConfigChanged` 里重建依赖。
- **标签/提示是 `Func<string>`。** 来自 ModTranslationClassBuilder 生成的强类型 `I18n` 访问器（键漏写会在编译期报错）。渲染期每次取值，故语言切换即时生效。需要具体字符串时调用它（`I18n.Config_X_Name()`），需要传给选项方法时直接传访问器。
- **枚举渲染按成员名走。** `AddEnumOption` 以文本选项呈现，值用成员名映射、可经 `formatValue` 本地化、可经 `allowedValues` 决定可选顺序与子集。枚举成员本身仍是强类型枚举，不落成字符串键。
- **字典成员不能走成员绑定。** 字典下标读写编成 `get_Item`/`set_Item` 方法调用，不是可赋值的公共属性链，`ConfigMember.CreateAccessor` 不接受。要渲染字典（如 LazyMod 按成长阶段存开关的 `Dictionary<int,bool>`）必须用 `AddCustomSection` 逃生舱按原始 GMCM API 逐个注册（LazyMod 的 `AddTreeSettingsPage` 即此用法）。
- **`KeybindList` 用专用选项。** 按键绑定列表只能经 `AddKeybindListOption`（底层 `AddKeybindList`），不要试图按文本/枚举处理。

---

# 6. 更新约定

本文是 PiCore 配置框架（`PiCore/Config`）的使用说明书（导航 + 使用约定），源码是唯一事实来源。改动公共 API 时请保持本文档同步：

- **新增/改名/删除** `PiCore.Config` 下任何公共类型或成员 → 同步更新对应章节与文末速查表。
- **行为契约变化**（如成员绑定规则、重置语义）→ 就地修订相关描述，禁止打补丁式追加。
- 验证状态随实际使用更新（当前无未验证类型；若新增类型先标 ⚠️，有消费方后移 ✅，同步改文首「验证状态说明」表与文末速查表）。
- changelog（`CHANGELOG.md` / `CHANGELOG.zh.md`）只记用户可见净变化，本文档记使用约定——两条线内容一致但不重复。

---

# 附录：类型速查表

| 类型 | 状态 | 一句话 |
| --- | --- | --- |
| `ConfigService<TConfig>` | ✅ | 生命周期：读取（自愈）/ 注册 / 保存 / 重置 / `onConfigChanged` 回调 |
| `ConfigMenuDescriptor<TConfig>` | ✅ | 声明式菜单：链式 `Add*` 描述，渲染映射到 GMCM |
| `ConfigMenuSection<TConfig, TSection>` | ✅ | 嵌套子配置作分区的构建器（选项方法对描述器一一对应） |
| `ConfigMember` | ✅ | internal：表达式树绑定 / 嵌套成员路径拼接 |
| `AddBoolOption` / `AddBoolSection` | ✅ | 布尔开关 / 布尔领头的分区 |
| `AddNumberOption` | ✅ | 整数或浮点数值（min/max/interval/formatValue） |
| `AddTextOption` | ✅ | 文本或下拉（allowedValues/formatAllowedValue） |
| `AddEnumOption` | ✅ | 枚举（文本选项呈现，可本地化） |
| `AddKeybindListOption` | ✅ | 按键绑定列表 |
| `AddSectionTitle` / `AddParagraph` | ✅ | 分区标题 / 段落 |
| `AddPage` / `AddPageLink` | ✅ | 多页菜单 / 页间跳转链接 |
| `AddSection` | ✅ | 嵌套配置作分区 |
| `AddCustomSection` | ✅ | 逃生舱：原始 GMCM API |