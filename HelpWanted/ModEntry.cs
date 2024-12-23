using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted;

internal class ModEntry : Mod
{
    private QuestManager questManager = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        this.questManager = new QuestManager(helper);
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        ModConfig.Init(helper);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        // 注册Harmony补丁
        var patches = new List<IPatcher>
        {
            new BillboardPatcher(), new ItemDeliveryQuestPatcher(), new SlayMonsterQuestPatcher(), new ResourceCollectionQuestPatcher(),
            new FishingQuestPatcher(), new Game1Patcher(), new TownPatcher()
        };
        if (helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage")) patches.Add(new RSVQuestBoardPatcher());
        HarmonyPatcher.Apply(this, patches.ToArray());
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        this.questManager.InitVanillaQuestList();
        if (this.Helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage") && ModConfig.Instance.EnableRSVQuestBoard) this.questManager.InitRSVQuestList();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForHelpWanted(
            this.Helper,
            this.ModManifest,
            () => ModConfig.Instance,
            () =>
            {
                ModConfig.Instance = new ModConfig();
                this.Helper.WriteConfig(ModConfig.Instance);
            },
            () => this.Helper.WriteConfig(ModConfig.Instance)
        ).Register();
    }
}