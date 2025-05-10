using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

public static class OptionFactory
{
    private static IModHelper helper = null!;
    private static readonly Dictionary<OptionId, Func<BaseOption>> Options = new();

    public static void Init(IModHelper _helper)
    {
        helper = _helper;
        // Beach
        Options[OptionId.Bobber] = () => new BobberOption();
        Options[OptionId.DecorationBoat] = () => new DecorationBoatOption();
        Options[OptionId.MagicBoat] = () => new MagicBoatOption();
        Options[OptionId.NightMarketTraveler] = () => new NightMarketTraveler();
        Options[OptionId.Willy] = () => new WillyOption();
        // Desert
        Options[OptionId.BuyQiCoins] = () => new BuyQiCoinsOption();
        Options[OptionId.Casino] = () => new CasinoOption();
        Options[OptionId.ClubSeller] = () => new ClubSellerOption();
        Options[OptionId.DesertTrade] = () => new DesertTradeOption();
        Options[OptionId.FarmerFile] = () => new FarmerFileOption();
        Options[OptionId.Sandy] = () => new SandyOption();
        // Farm
        Options[OptionId.ShippingBin] = () => new ShippingBinOption(helper);
        Options[OptionId.TV] = () => new TVOption(helper);
        // Forest
        Options[OptionId.HatMouse] = () => new HatMouseOption();
        Options[OptionId.Marnie] = () => new MarnieOption(helper);
        Options[OptionId.Raccoon] = () => new RaccoonOption(helper);
        Options[OptionId.Traveler] = () => new TravelerOption();
        Options[OptionId.Wizard] = () => new WizardOption();
        // GingerIsland
        Options[OptionId.ForgeOption] = () => new ForgeOption();
        Options[OptionId.IslandResort] = () => new IslandResortOption();
        Options[OptionId.IslandTrade] = () => new IslandTradeOption();
        Options[OptionId.QiCat] = () => new QiCatOption(helper);
        Options[OptionId.QiGemShop] = () => new QiGemShopOption();
        Options[OptionId.QiSpecialOrder] = () => new QiSpecialOrderOption();
        Options[OptionId.VolcanoShop] = () => new VolcanoShopOption();
        // Mountain
        Options[OptionId.Dwarf] = () => new DwarfOption();
        Options[OptionId.Marlon] = () => new MarlonOption();
        Options[OptionId.Monster] = () => new MonsterOption(helper);
        Options[OptionId.Robin] = () => new RobinOption();
        // RSV
        Options[OptionId.Ian] = () => new IanOption();
        Options[OptionId.Jeric] = () => new JericOption();
        Options[OptionId.Joi] = () => new JoiOption();
        Options[OptionId.Kimpoi] = () => new KimpoiOption();
        Options[OptionId.Lola] = () => new LolaOption();
        Options[OptionId.Lorenzo] = () => new LorenzoOption();
        Options[OptionId.MysticFalls1] = () => new MysticFalls1Option();
        Options[OptionId.MysticFalls2] = () => new MysticFalls2Option();
        Options[OptionId.MysticFalls3] = () => new MysticFalls3Option();
        Options[OptionId.NinjaBoard] = () => new NinjaBoardOption();
        Options[OptionId.Paula] = () => new PaulaOption();
        Options[OptionId.Pika] = () => new PikaOption();
        Options[OptionId.RSVQuestBoard] = () => new RSVQuestBoardOption();
        Options[OptionId.RSVSpecialOrder] = () => new RSVSpecialOrderOption();
        // SVE
        Options[OptionId.Sophia] = () => new SophiaOption();
        // Town
        Options[OptionId.AbandonedJojaMart] = () => new AbandonedJojaMartOption();
        Options[OptionId.Billboard] = () => new BillboardOption();
        Options[OptionId.Bookseller] = () => new BooksellerOption();
        Options[OptionId.Clint] = () => new ClintOption();
        Options[OptionId.CommunityCenter] = () => new CommunityCenterOption();
        Options[OptionId.Dye] = () => new DyeOption();
        Options[OptionId.Gus] = () => new GusOption();
        Options[OptionId.HarveyOption] = () => new HarveyOption();
        Options[OptionId.IceCreamStand] = () => new IceCreamStandOption();
        Options[OptionId.JojaShop] = () => new JojaShopOption();
        Options[OptionId.Krobus] = () => new KrobusOption();
        Options[OptionId.Pierre] = () => new PierreOption();
        Options[OptionId.PrizeTicket] = () => new PrizeTicketOption();
        Options[OptionId.SpecialOrder] = () => new SpecialOrderOption();
        Options[OptionId.Statue] = () => new StatueOption();
        Options[OptionId.Tailoring] = () => new TailoringOption();
    }

    private static BaseOption CreateOption(OptionId optionId)
    {
        return Options[optionId].Invoke();
    }

    public static BaseOption[] CreateBeachOptions()
    {
        return new[]
        {
            CreateOption(OptionId.Willy),
            CreateOption(OptionId.Bobber),
            CreateOption(OptionId.NightMarketTraveler),
            CreateOption(OptionId.DecorationBoat),
            CreateOption(OptionId.MagicBoat)
        };
    }

    public static BaseOption[] CreateDesertOptions()
    {
        return new[]
        {
            CreateOption(OptionId.Sandy),
            CreateOption(OptionId.DesertTrade),
            CreateOption(OptionId.Casino),
            CreateOption(OptionId.FarmerFile),
            CreateOption(OptionId.BuyQiCoins),
            CreateOption(OptionId.ClubSeller)
        };
    }

    public static BaseOption[] CreateFarmOptions()
    {
        return new[]
        {
            CreateOption(OptionId.TV),
            CreateOption(OptionId.ShippingBin)
        };
    }

    public static BaseOption[] CreateForestOptions()
    {
        return new[]
        {
            CreateOption(OptionId.Marnie),
            CreateOption(OptionId.Traveler),
            CreateOption(OptionId.HatMouse),
            CreateOption(OptionId.Wizard),
            CreateOption(OptionId.Raccoon)
        };
    }

    public static BaseOption[] CreateGingerIslandOptions()
    {
        return new[]
        {
            CreateOption(OptionId.QiSpecialOrder),
            CreateOption(OptionId.QiGemShop),
            CreateOption(OptionId.QiCat),
            CreateOption(OptionId.IslandTrade),
            CreateOption(OptionId.IslandResort),
            CreateOption(OptionId.VolcanoShop),
            CreateOption(OptionId.ForgeOption)
        };
    }

    public static BaseOption[] CreateMountainOptions()
    {
        return new[]
        {
            CreateOption(OptionId.Robin),
            CreateOption(OptionId.Dwarf),
            CreateOption(OptionId.Monster),
            CreateOption(OptionId.Marlon)
        };
    }

    public static BaseOption[] CreateRSVOptions()
    {
        return new[]
        {
            CreateOption(OptionId.RSVQuestBoard),
            CreateOption(OptionId.RSVSpecialOrder),
            CreateOption(OptionId.Ian),
            CreateOption(OptionId.Paula),
            CreateOption(OptionId.Lorenzo),
            CreateOption(OptionId.Jeric),
            CreateOption(OptionId.Kimpoi),
            CreateOption(OptionId.Pika),
            CreateOption(OptionId.Lola),
            CreateOption(OptionId.NinjaBoard),
            CreateOption(OptionId.Joi),
            CreateOption(OptionId.MysticFalls1),
            CreateOption(OptionId.MysticFalls2),
            CreateOption(OptionId.MysticFalls3)
        };
    }

    public static BaseOption[] CreateSVEOptions()
    {
        return new[]
        {
            CreateOption(OptionId.Sophia)
        };
    }

    public static BaseOption[] CreateTownOptions()
    {
        return new[]
        {
            CreateOption(OptionId.Billboard),
            CreateOption(OptionId.SpecialOrder),
            // CreateOption(OptionId.CommunityCenter),
            CreateOption(OptionId.Pierre),
            CreateOption(OptionId.Clint),
            CreateOption(OptionId.Gus),
            CreateOption(OptionId.JojaShop),
            CreateOption(OptionId.PrizeTicket),
            CreateOption(OptionId.Bookseller),
            CreateOption(OptionId.Krobus),
            CreateOption(OptionId.Statue),
            CreateOption(OptionId.HarveyOption),
            CreateOption(OptionId.Tailoring),
            CreateOption(OptionId.Dye),
            CreateOption(OptionId.IceCreamStand),
            CreateOption(OptionId.AbandonedJojaMart)
        };
    }

    public static BaseOption[] CreateFavoriteOptions()
    {
        return ModConfig.Instance.FavoriteMenus.Select(CreateOption).ToArray();
    }
}