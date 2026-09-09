using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Config;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Helper;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.UI;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

/// <summary>
/// 声明式选项目录：页签顺序、页签标题、每页签选项的**唯一事实源**。
/// 页签与选项的身份均为字符串 id（页签 id 与旧 <c>MenuTabId</c> 枚举成员名逐字一致，
/// 选项 id 大多与旧 <c>OptionId</c> 枚举成员名一致）——
/// SMAPI 以 <c>StringEnumConverter</c> 序列化配置，枚举本就以名字落盘，故老 config.json 的
/// 收藏列表与默认页签读到的新字符串值大多与旧值兼容。
/// <b>例外（破坏性）</b>：<c>HarveyOption</c>/<c>ForgeOption</c> 两条去掉了 <c>Option</c> 后缀改名为
/// <c>Harvey</c>/<c>Forge</c>（与其余选项 id 命名对齐），老配置里这两项收藏/默认值会失配，需重新设置。
/// 每行声明一个选项的完整信息——身份、所属页签、标签访问器、图标序号与行为工厂；
/// 物化（<see cref="CreateTabOptions" />/<see cref="CreateFavoriteOptions" />）在目录层
/// 把行解析成 <see cref="OptionEntry" />（解析纹理表与源区域、调用工厂构造行为实例）。
/// <see cref="MainMenuView" /> 的整套页签渲染路径（顺序/标题/选项）都由本目录物化，
/// 不再在视图层散落各自的映射。Favorite 页签不属于目录——它是运行时聚合（由
/// <see cref="ModConfig.FavoriteMenus" /> 驱动），物化时作为固定第一位的特殊页签注入，
/// 内容继续走现有 <see cref="FavoriteTabContent" /> 每次重建逻辑。
/// SVE/RSV 页签以可见性谓词按对应模组的安装状态条件挂载。
/// </summary>
internal static class OptionCatalog
{
    // 页签 id 词表：与旧 MenuTabId 成员名逐字一致，作为配置菜单「默认页签」与纹理表共用的字符串身份。
    internal const string FavoriteTabId = "Favorite";
    internal const string FarmTabId = "Farm";
    internal const string TownTabId = "Town";
    internal const string MountainTabId = "Mountain";
    internal const string ForestTabId = "Forest";
    internal const string BeachTabId = "Beach";
    internal const string DesertTabId = "Desert";
    internal const string GingerIslandTabId = "GingerIsland";
    internal const string SveTabId = "SVE";
    internal const string RsvTabId = "RSV";

    /// <summary>SVE 模组唯一标识。</summary>
    private const string SveModId = "FlashShifter.SVECode";

    /// <summary>RSV 模组唯一标识。</summary>
    private const string RsvModId = "Rafseazz.RidgesideVillage";

    /// <summary>图标单元边长（每张位置图为 200×200 的单元，横向每行 3 个）。</summary>
    private const int IconCellSize = 200;

    /// <summary>每行图标列数（决定图标序号的源区域换算）。</summary>
    private const int IconColumnsPerRow = 3;

    private static IModHelper helper = null!;

    /// <summary>页签目录（声明顺序即页签顺序；Favorite 由物化层注入，不在此列）。</summary>
    private static readonly IReadOnlyList<TabDescriptor> Tabs = new TabDescriptor[]
    {
        new(FarmTabId, I18n.UI_Tab_Farm),
        new(TownTabId, I18n.UI_Tab_Town),
        new(MountainTabId, I18n.UI_Tab_Mountain),
        new(ForestTabId, I18n.UI_Tab_Forest),
        new(BeachTabId, I18n.UI_Tab_Beach),
        new(DesertTabId, I18n.UI_Tab_Desert),
        new(GingerIslandTabId, I18n.UI_Tab_GingerIsland),
        new(SveTabId, I18n.UI_Tab_SVE, h => h.ModRegistry.Get(SveModId) is not null),
        new(RsvTabId, I18n.UI_Tab_RSV, h => h.ModRegistry.Get(RsvModId) is not null)
    };

    // 选项目录：每行声明一个选项的完整信息（身份 + 所属页签 + 标签访问器 + 图标序号 + 行为工厂），
    // 声明顺序即该页签内的显示顺序；图标序号是显式每行数据，调整行序不会改变图标。
    // 6 个依赖 helper 的选项以闭包工厂承载（与重构前构造形态一致）。
    private static readonly IReadOnlyList<OptionDescriptor> Options = new OptionDescriptor[]
    {
        // Farm
        new(FarmTabId, "TV", I18n.UI_Option_TV, 0, () => new TVOption(helper)),
        new(FarmTabId, "ShippingBin", I18n.UI_Option_ShippingBin, 1, () => new ShippingBinOption(helper)),

        // Town
        new(TownTabId, "Billboard", I18n.UI_Option_Billboard, 0, () => new BillboardOption()),
        new(TownTabId, "SpecialOrder", I18n.UI_Option_SpecialOrder, 1, () => new SpecialOrderOption()),
        new(TownTabId, "CommunityCenter", I18n.UI_Option_CommunityCenter, 2, () => new CommunityCenterOption()),
        new(TownTabId, "Pierre", I18n.UI_Option_Pierre, 3, () => new PierreOption()),
        new(TownTabId, "Clint", I18n.UI_Option_Clint, 4, () => new ClintOption()),
        new(TownTabId, "Gus", I18n.UI_Option_Gus, 5, () => new GusOption()),
        new(TownTabId, "JojaShop", I18n.UI_Option_JojaShop, 6, () => new JojaShopOption()),
        new(TownTabId, "PrizeTicket", I18n.UI_Option_PrizeTicket, 7, () => new PrizeTicketOption()),
        new(TownTabId, "Bookseller", I18n.UI_Option_Bookseller, 8, () => new BooksellerOption()),
        new(TownTabId, "Krobus", I18n.UI_Option_Krobus, 9, () => new KrobusOption()),
        new(TownTabId, "Statue", I18n.UI_Option_Statue, 10, () => new StatueOption()),
        new(TownTabId, "Harvey", I18n.UI_Option_Harvey, 11, () => new HarveyOption()),
        new(TownTabId, "Tailoring", I18n.UI_Option_Tailoring, 12, () => new TailoringOption()),
        new(TownTabId, "Dye", I18n.UI_Option_Dye, 13, () => new DyeOption()),
        new(TownTabId, "IceCreamStand", I18n.UI_Option_IceCreamStand, 14, () => new IceCreamStandOption()),
        new(TownTabId, "AbandonedJojaMart", I18n.UI_Option_AbandonedJojaMart, 15, () => new AbandonedJojaMartOption()),

        // Mountain
        new(MountainTabId, "Robin", I18n.UI_Option_Robin, 0, () => new RobinOption()),
        new(MountainTabId, "Dwarf", I18n.UI_Option_Dwarf, 1, () => new DwarfOption()),
        new(MountainTabId, "Monster", I18n.UI_Option_Monster, 2, () => new MonsterOption(helper)),
        new(MountainTabId, "Marlon", I18n.UI_Option_Marlon, 3, () => new MarlonOption()),

        // Forest
        new(ForestTabId, "Marnie", I18n.UI_Option_Marnie, 0, () => new MarnieOption(helper)),
        new(ForestTabId, "Traveler", I18n.UI_Option_Traveler, 1, () => new TravelerOption()),
        new(ForestTabId, "HatMouse", I18n.UI_Option_HatMouse, 2, () => new HatMouseOption()),
        new(ForestTabId, "Wizard", I18n.UI_Option_Wizard, 3, () => new WizardOption()),
        new(ForestTabId, "Raccoon", I18n.UI_Option_Raccoon, 4, () => new RaccoonOption(helper)),

        // Beach
        new(BeachTabId, "Willy", I18n.UI_Option_Willy, 0, () => new WillyOption()),
        new(BeachTabId, "Bobber", I18n.UI_Option_Bobber, 1, () => new BobberOption()),
        new(BeachTabId, "NightMarketTraveler", I18n.UI_Option_NightMarketTraveler, 2, () => new NightMarketTravelerOption()),
        new(BeachTabId, "DecorationBoat", I18n.UI_Option_DecorationBoat, 3, () => new DecorationBoatOption()),
        new(BeachTabId, "MagicBoat", I18n.UI_Option_MagicBoat, 4, () => new MagicBoatOption()),

        // Desert
        new(DesertTabId, "Sandy", I18n.UI_Option_Sandy, 0, () => new SandyOption()),
        new(DesertTabId, "DesertTrade", I18n.UI_Option_DesertTrade, 1, () => new DesertTradeOption()),
        new(DesertTabId, "Casino", I18n.UI_Option_Casino, 2, () => new CasinoOption()),
        new(DesertTabId, "FarmerFile", I18n.UI_Option_FarmerFile, 3, () => new FarmerFileOption()),
        new(DesertTabId, "BuyQiCoins", I18n.UI_Option_BuyQiCoins, 4, () => new BuyQiCoinsOption()),
        new(DesertTabId, "ClubSeller", I18n.UI_Option_ClubSeller, 5, () => new ClubSellerOption()),

        // GingerIsland
        new(GingerIslandTabId, "QiSpecialOrder", I18n.UI_Option_QiSpecialOrder, 0, () => new QiSpecialOrderOption()),
        new(GingerIslandTabId, "QiGemShop", I18n.UI_Option_QiGemShop, 1, () => new QiGemShopOption()),
        new(GingerIslandTabId, "QiCat", I18n.UI_Option_QiCat, 2, () => new QiCatOption(helper)),
        new(GingerIslandTabId, "IslandTrade", I18n.UI_Option_IslandTrade, 3, () => new IslandTradeOption()),
        new(GingerIslandTabId, "IslandResort", I18n.UI_Option_IslandResort, 4, () => new IslandResortOption()),
        new(GingerIslandTabId, "VolcanoShop", I18n.UI_Option_VolcanoShop, 5, () => new VolcanoShopOption()),
        new(GingerIslandTabId, "Forge", I18n.UI_Option_Forge, 6, () => new ForgeOption()),

        // SVE
        new(SveTabId, "Sophia", I18n.UI_Option_Sophia, 0, () => new SophiaOption()),

        // RSV
        new(RsvTabId, "RSVQuestBoard", I18n.UI_Option_RSVQuestBoard, 0, () => new RSVQuestBoardOption()),
        new(RsvTabId, "RSVSpecialOrder", I18n.UI_Option_RSVSpecialOrder, 1, () => new RSVSpecialOrderOption()),
        new(RsvTabId, "Ian", I18n.UI_Option_Ian, 2, () => new IanOption()),
        new(RsvTabId, "Paula", I18n.UI_Option_Paula, 3, () => new PaulaOption()),
        new(RsvTabId, "Lorenzo", I18n.UI_Option_Lorenzo, 4, () => new LorenzoOption()),
        new(RsvTabId, "Jeric", I18n.UI_Option_Jeric, 5, () => new JericOption()),
        new(RsvTabId, "Kimpoi", I18n.UI_Option_Kimpoi, 6, () => new KimpoiOption()),
        new(RsvTabId, "Pika", I18n.UI_Option_Pika, 7, () => new PikaOption()),
        new(RsvTabId, "Lola", I18n.UI_Option_Lola, 8, () => new LolaOption()),
        new(RsvTabId, "NinjaBoard", I18n.UI_Option_NinjaBoard, 10, () => new NinjaBoardOption()),
        new(RsvTabId, "Joi", I18n.UI_Option_Joi, 11, () => new JoiOption()),
        new(RsvTabId, "MysticFalls1", I18n.UI_Option_MysticFalls1, 12, () => new MysticFallsOption("RSVMysticFalls1")),
        new(RsvTabId, "MysticFalls2", I18n.UI_Option_MysticFalls2, 13, () => new MysticFallsOption("RSVMysticFalls2")),
        new(RsvTabId, "MysticFalls3", I18n.UI_Option_MysticFalls3, 14, () => new MysticFallsOption("RSVMysticFalls3"))
    };

    /// <summary>初始化目录（仅持有依赖：SVE/RSV 可见性谓词与 helper 闭包工厂使用）。</summary>
    /// <param name="_helper">SMAPI 帮助器。</param>
    public static void Init(IModHelper _helper)
    {
        helper = _helper;
    }

    /// <summary>推导页签显示顺序：收藏页签固定第一位，其余按目录声明顺序过滤掉不可见页签。</summary>
    /// <returns>可见页签 id 列表。</returns>
    public static IReadOnlyList<string> GetTabOrder()
    {
        var order = new List<string> { FavoriteTabId };
        order.AddRange(
            Tabs
                .Where(tab => tab.Visible?.Invoke(helper) ?? true)
                .Select(tab => tab.Id)
        );

        return order;
    }

    /// <summary>
    /// 取全部可选页签 id（Favorite + 目录声明全部页签，不过滤安装状态）。
    /// 供配置菜单「默认页签」下拉使用：与 <see cref="GetTabOrder" />（菜单可见顺序）不同，
    /// 此处始终包含条件挂载的 SVE/RSV，保证老配置里的任一旧值都在可选列表内、无需迁移。
    /// </summary>
    /// <returns>全部可选页签 id 列表。</returns>
    public static string[] GetAllTabIds()
    {
        var ids = new List<string> { FavoriteTabId };
        ids.AddRange(Tabs.Select(tab => tab.Id));

        return ids.ToArray();
    }

    /// <summary>取页签标题文案（收藏页签为目录注入的特殊页签，标题一并由目录给出）。</summary>
    /// <param name="tabId">页签 id。</param>
    /// <returns>页签标题；未知页签 id 原样返回（配置菜单对旧值兜底）。</returns>
    public static string GetTabTitle(string tabId)
    {
        if (tabId == FavoriteTabId)
        {
            return I18n.UI_Tab_Favorites();
        }

        return Tabs.FirstOrDefault(tab => tab.Id == tabId)?.Title() ?? tabId;
    }

    /// <summary>按页签物化其选项（声明顺序即显示顺序；Favorite 页签不属于目录，勿查询）。</summary>
    /// <param name="tabId">页签 id。</param>
    /// <returns>该页签的选项条目。</returns>
    public static OptionEntry[] CreateTabOptions(string tabId)
    {
        return Options
            .Where(option => option.Tab == tabId)
            .Select(Materialize)
            .ToArray();
    }

    /// <summary>按当前收藏列表物化收藏页签的选项（收藏走旧路径，仍由目录行驱动）。</summary>
    /// <returns>收藏的选项条目。</returns>
    public static OptionEntry[] CreateFavoriteOptions()
    {
        var favorites = new List<OptionEntry>();

        foreach (var favoriteId in ModConfig.Instance.FavoriteMenus)
        {
            var option = Options.FirstOrDefault(row => row.Id == favoriteId);

            if (option is not null)
            {
                favorites.Add(Materialize(option));
            }
        }

        return favorites.ToArray();
    }

    /// <summary>把目录行物化为可消费条目：解析纹理与源区域、调用工厂构造行为实例、解析标签。</summary>
    /// <param name="option">目录行。</param>
    /// <returns>物化后的选项条目。</returns>
    private static OptionEntry Materialize(OptionDescriptor option)
    {
        return new OptionEntry(
            option.Id,
            option.Label(),
            TextureManager.Instance.GetTexture(option.Tab),
            GetSourceRectangle(option.IconIndex),
            option.Factory()
        );
    }

    /// <summary>图标序号 → 纹理表源区域：每行 3 个 200×200 单元，向下逐行排布。</summary>
    /// <param name="iconIndex">图标序号（目录行显式数据）。</param>
    /// <returns>源区域。</returns>
    private static Rectangle GetSourceRectangle(int iconIndex)
    {
        var column = iconIndex % IconColumnsPerRow;
        var row = iconIndex / IconColumnsPerRow;

        return new Rectangle(column * IconCellSize, row * IconCellSize, IconCellSize, IconCellSize);
    }

    /// <summary>页签行声明：标识、标题访问器、可选可见性谓词（null 表示恒可见）。</summary>
    /// <param name="Id">页签 id。</param>
    /// <param name="Title">标题访问器。</param>
    /// <param name="Visible">可见性谓词。</param>
    private sealed record TabDescriptor(string Id, Func<string> Title, Func<IModHelper, bool>? Visible = null);

    /// <summary>选项行声明：身份、所属页签、标签访问器、图标序号与行为工厂（显示顺序由声明相对位置决定）。</summary>
    /// <param name="Tab">所属页签。</param>
    /// <param name="Id">选项身份（字符串 id）。</param>
    /// <param name="Label">标签访问器。</param>
    /// <param name="IconIndex">图标序号（显式数据，与页签内顺序无关）。</param>
    /// <param name="Factory">行为实例工厂。</param>
    private sealed record OptionDescriptor(
        string Tab,
        string Id,
        Func<string> Label,
        int IconIndex,
        Func<BaseOption> Factory
    );
}
