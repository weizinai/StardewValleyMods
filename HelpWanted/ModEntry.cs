using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Manager;
using weizinai.StardewValleyMod.HelpWanted.Menu;
using weizinai.StardewValleyMod.HelpWanted.Patcher;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted;

internal class ModEntry : Mod
{
    public static bool IsRSVLoaded;
    public static Random Random { get; } = new();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger.Init(this.Monitor);
        TextureManager.Instance.Init(helper);
        ModConfig.Init(helper);

        IsRSVLoaded = this.Helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage");

        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;

        // 注册Harmony补丁
        var patches = new List<IPatcher>
        {
            new BillboardPatcher(),
            new QuestPatcher(),
            new ItemDeliveryQuestPatcher(),
            new SlayMonsterQuestPatcher(),
            new ResourceCollectionQuestPatcher(),
            new FishingQuestPatcher(),
            new Game1Patcher(),
            new TownPatcher()
        };
        if (IsRSVLoaded) patches.Add(new RSVQuestBoardPatcher());
        HarmonyPatcher.Apply(this.ModManifest.UniqueID, patches.ToArray());
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        this.ClearCache();

        VanillaQuestManager.Instance.InitVanillaQuestList();
        if (IsRSVLoaded && ModConfig.Instance.RSVConfig.EnableRSVQuestBoard)
        {
            RSVQuestManager.Instance.InitRSVQuestList();
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            new GenericModConfigMenuIntegrationForHelpWanted(),
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
    }

    private void ClearCache()
    {
        QuestItemManager.Instance.ClearCache();
        QuestMonsterManager.Instance.ClearCache();
        VanillaQuestManager.Instance.ClearCache();
        RSVQuestManager.Instance.ClearCache();
        BaseQuestBoard.ClearCache();
    }
}