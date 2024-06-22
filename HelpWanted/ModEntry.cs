using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.Common.Patcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Patcher;
using weizinai.StardewValleyMod.HelpWanted.Patches;

namespace weizinai.StardewValleyMod.HelpWanted;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private QuestManager questManager = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        questManager = new QuestManager(config, helper);
        Log.Init(Monitor);
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        // 注册Harmony补丁
        var patches = new List<IPatcher>
        {
            new BillboardPatcher(config), new ItemDeliveryQuestPatcher(config), new SlayMonsterQuestPatcher(config), new ResourceCollectionQuestPatcher(config),
            new FishingQuestPatcher(config), new Game1Patcher(), new TownPatcher()
        };
        if (helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage")) patches.Add(new RSVQuestBoardPatcher(config));
        HarmonyPatcher.Apply(this, patches.ToArray());
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        questManager.InitVanillaQuestList();
        if (Helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage") && config.EnableRSVQuestBoard)
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