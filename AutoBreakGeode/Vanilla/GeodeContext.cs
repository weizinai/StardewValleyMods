using StardewValley;
using StardewValley.Menus;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Vanilla;

/// <summary>
/// 本帧的晶球菜单现场：当前打开的是不是晶球菜单、手里拿的是不是晶球。
/// </summary>
/// <remarks>
/// 「哪条菜单算晶球菜单」原先在处理器里判三次、写两种（<c>is not GeodeMenu</c> 与 <c>as GeodeMenu</c> 各一处，
/// 切换开关时再判一次），现在收成本类里唯一的一次转型：调用方拿到的是判据的结论，不再各自复述。
/// 自动砸与叠层共用同一份现场——处理器每帧只捕一次，既交给会话决策，也用于叠层的启停与锚位，
/// 故叠层的可见性与自动砸的上下文永远同源。
/// 只读快照、不缓存跨帧结果：菜单身份与手持物下一帧都可能已经变了，本值的含义仅限捕获它的那一帧。
/// 判定的事实仍由 <see cref="GeodeMenuContract" /> 回答，本类只把它的结论与本帧的菜单打包成一个值；原版升级要重新核对的
/// 字段与时序仍然集中在 <c>Vanilla/</c> 这一处。
/// </remarks>
internal readonly struct GeodeContext
{
    /// <summary>当前打开的晶球菜单；没有菜单、或当前打开的是别的菜单时为 <c>null</c>。</summary>
    public GeodeMenu? Menu { get; }

    /// <summary>原版此刻认定手里拿着的是能砸的晶球（含金色椰子、谜之盒等同类物）。</summary>
    public bool IsHoldingGeode { get; }

    /// <summary>
    /// 构造现场值：只经 <see cref="Capture" /> 捕获，不对外开放——现场只能来自游戏此刻的事实。
    /// 结构体的 <c>default</c> 不构成漏洞：它等价于「不在晶球菜单里」（菜单为 <c>null</c>、手持为 false），与真的捕获结果一致。
    /// </summary>
    /// <param name="menu">当前打开的晶球菜单，可为 <c>null</c>。</param>
    /// <param name="isHoldingGeode">手里拿的是不是晶球。</param>
    private GeodeContext(GeodeMenu? menu, bool isHoldingGeode)
    {
        this.Menu = menu;
        this.IsHoldingGeode = isHoldingGeode;
    }

    /// <summary>按游戏此刻的事实捕获现场：读当前活动菜单判是不是晶球菜单，再判手里拿的是不是晶球。</summary>
    /// <returns>本帧的现场值；不在晶球菜单里时 <see cref="Menu" /> 为 <c>null</c>。</returns>
    public static GeodeContext Capture()
    {
        var menu = Game1.activeClickableMenu as GeodeMenu;

        // 菜单不在时不必问手持：没有晶球菜单就没有「手里那颗」，两处判据都只以菜单已在为前提
        return new GeodeContext(menu, menu is not null && GeodeMenuContract.IsHoldingGeode(menu));
    }
}
