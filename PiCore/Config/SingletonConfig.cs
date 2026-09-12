namespace weizinai.StardewValleyMod.PiCore.Config;

/// <summary>配置根的单例槽位：模组的根配置类继承它即得到静态的 <see cref="Instance" /> 入口，读盘（含损坏自愈）、写盘与重置由 <see cref="ConfigService{TConfig}" /> 承担。</summary>
/// <typeparam name="TConfig">配置类自身，写成 <c>internal class ModConfig : SingletonConfig&lt;ModConfig&gt;</c>。</typeparam>
public abstract class SingletonConfig<TConfig> where TConfig : SingletonConfig<TConfig>
{
    /// <summary>
    /// 框架读写的配置实例：在模组 <c>Entry</c> 里构造 <see cref="ConfigService{TConfig}" /> 时写入，之前读到的是 <c>null</c>。
    /// 写权只归框架，重置也保持引用不变；只有模组的根配置可以继承本类——嵌套配置若也继承，它的 <see cref="Instance" /> 会是与根配置实例无关的另一份数据，静默脱钩。
    /// </summary>
    public static TConfig Instance { get; internal set; } = null!;
}
