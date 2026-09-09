using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class CommunityCenterOption : BaseOption
{
    public override bool IsEnable()
    {
        // 保留 Joja 路线与森林视野剧情的双重进度门控
        return !Game1.player.mailReceived.Contains("JojaMember") && Game1.player.mailReceived.Contains("canReadJunimoText");
    }

    public override void Apply()
    {
        // 走原版「背包页社区中心图标」同款入口（JunimoNoteMenu(fromGameMenu: true)）从任意地点打开：
        // 构造时自动选中首个仍有可完成束的房间并带房间切换箭头；setUpMenu 会登记
        // seenJunimoNote/wizardJunimoNote 邮件——与真的走进社区中心读到告示的登记一致。
        // 该入口与背包页图标同为只读（构造器把各 bundle 的 depositsAllowed 置 false），远程完成束不在本入口
        // 语义内；弃用旧对话框管线与 CommunityCenter.checkBundle（后者走 bundleMutexes[area].RequestLock，
        // 是「站在告示板前按动作键」的路径；fromGameMenu 变体在 SkillsPage/InventoryPage 原版路径同为无
        // mutex 直开，故不套多人锁）。
        Game1.activeClickableMenu = new JunimoNoteMenu(fromGameMenu: true);
    }
}
