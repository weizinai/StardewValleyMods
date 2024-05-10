using Common.Integrations;
using Common.Patch;
using FriendshipDecayModify.Framework;
using FriendshipDecayModify.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FriendshipDecayModify;

public class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        // 注册Harmony补丁
        HarmonyPatcher.Patch(this, new GameLocationPatcher(config), new FarmerPatcher(config), new NPCPatcher(config), new FarmerPatcher(config));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
        if (configMenu is null) return;

        configMenu.Register(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));

        // 每日对话修改
        configMenu.AddSectionTitle(ModManifest, I18n.Config_DailyGreetingModifyTitle_Name);
        configMenu.AddNumberOption(
            ModManifest,
            () => config.DailyGreetingModifyForVillager,
            value => config.DailyGreetingModifyForVillager = value,
            I18n.Config_DailyGreetingModifyForVillager_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.DailyGreetingModifyForDatingVillager,
            value => config.DailyGreetingModifyForDatingVillager = value,
            I18n.Config_DailyGreetingModifyForDatingVillager_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.DailyGreetingModifyForSpouse,
            value => config.DailyGreetingModifyForSpouse = value,
            I18n.Config_DailyGreetingModifyForSpouse_Name
        );
        // 礼物修改
        configMenu.AddSectionTitle(ModManifest, I18n.Config_GiftModifyTitle_Name);
        configMenu.AddNumberOption(
            ModManifest,
            () => config.DislikeGiftModify,
            value => config.DislikeGiftModify = value,
            I18n.Config_DislikeGiftModify_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.HateGiftModify,
            value => config.HateGiftModify = value,
            I18n.Config_HateGiftModify_Name
        );
        // 垃圾桶修改
        configMenu.AddSectionTitle(ModManifest, I18n.Config_GarbageCanModify_Name);
        configMenu.AddNumberOption(
            ModManifest,
            () => config.GarbageCanModify,
            value => config.GarbageCanModify = value,
            I18n.Config_GarbageCanModify_Name
        );
        // 动物好感度修改
        configMenu.AddSectionTitle(ModManifest, I18n.Config_PetAnimalModifyForFriendship_Name);
        configMenu.AddNumberOption(
            ModManifest,
            () => config.PetAnimalModifyForFriendship,
            value => config.PetAnimalModifyForFriendship = value,
            I18n.Config_PetAnimalModifyForFriendship_Name
        );
        // 动物心情修改
        configMenu.AddSectionTitle(ModManifest, I18n.Config_PetAnimalModifyForHappiness_Name);
        configMenu.AddNumberOption(
            ModManifest,
            () => config.PetAnimalModifyForHappiness,
            value => config.PetAnimalModifyForHappiness = value,
            I18n.Config_PetAnimalModifyForHappiness_Name
        );
    }
}