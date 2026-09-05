# ModTranslationClassBuilder（翻译键强类型类生成器）— 本地参考

> 本地专用参考：2026-09-05 从 GitHub master README 快照，与 NuGet 包 `Pathoschild.Stardew.ModTranslationClassBuilder` v2.2.0 内置 README 逐字一致。**只作本地引用，不再联网更新**。

## 作用

由模组的 `i18n/default.json` 自动生成强类型静态类，把运行期才校验的翻译查找变成编译期校验：

```cs
// 原方式：键或变量名写错，要进游戏跑到对应代码才报错
string text = helper.Translation.Get("range-value", new { min = 1, max = 5 });
// 生成类方式：写错（如 I18n.RangeValues）立即编译报错
string text = I18n.RangeValue(min: 1, max: 5);
```

## 使用

1. 引用 NuGet 包 `Pathoschild.Stardew.ModTranslationClassBuilder`。
2. 在模组 `Entry` 方法里加一行：

   ```cs
   I18n.Init(helper.Translation);
   ```

3. `i18n/default.json` 变化时生成类自动更新；之后在模组代码任意位置直接用 `I18n.*`。

## 命名约定（键 → 方法）

| `i18n/default.json` 键 | 生成的方法 |
|---|---|
| `ready` | `I18n.Ready()` |
| `ready-now` | `I18n.ReadyNow()` |
| `generic.ready-now` | `I18n.Generic_ReadyNow()` |

规则：键转 CamelCase，`.` 变 `_` 作分组。类默认使用项目根命名空间。

## 定制（.csproj `<PropertyGroup>`，属性加 `TranslationClassBuilder_` 前缀）

```xml
<PropertyGroup>
  <TranslationClassBuilder_ClassName>Translations</TranslationClassBuilder_ClassName>
</PropertyGroup>
```

| 参数 | 说明 | 默认值 |
|---|---|---|
| `ClassName` | 生成类名 | `I18n` |
| `Namespace` | 生成类命名空间 | 项目根命名空间 |
| `ClassModifiers` | 生成类修饰符（如改 `public`） | `internal static` |
| `CreateBackup` | 是否在项目 `Generated` 子目录留备份（`false` 时生成文件隐藏且不入版本库） | `false` |