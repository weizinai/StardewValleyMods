using Common;
using Common.Patch;
using HelpWanted.Framework;
using HelpWanted.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace HelpWanted;

internal class ModEntry : Mod
{
    private static ModConfig config = new();
    private QuestManager questManager = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        _ = new Log(Monitor);
        config = helper.ReadConfig<ModConfig>();
        questManager = new QuestManager(helper, config);
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        // 注册Harmony补丁
        var patches = new List<IPatcher>
        {
            new BillboardPatcher(config), new ItemDeliveryQuestPatcher(config),
            new SlayMonsterQuestPatcher(config), new ResourceCollectionQuestPatcher(config), new FishingQuestPatcher(config), new Game1Patcher(), new TownPatcher()
        };
        if (helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage")) patches.Add(new RSVQuestBoardPatcher(config));
        HarmonyPatcher.Apply(this, patches.ToArray());
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (Game1.stats.DaysPlayed <= 1 && !config.QuestFirstDay)
        {
            Log.Trace("今天是游戏第一天,不生成任务.");
            return;
        }

        if ((Utility.isFestivalDay() || Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season)) && !config.QuestFestival)
        {
            Log.Trace("今天或明天是节日,不生成任务.");
            return;
        }

        if (Game1.random.NextDouble() >= config.DailyQuestChance)
        {
            Log.Trace("今天不生成任务.");
            return;
        }

        questManager.InitVanillaQuestList();
        if (Helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage"))
            questManager.InitRSVQuestList();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForHelpWanted(
            Helper,
            ModManifest,
            () => config,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        ).Register();
    }
}