using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.FriendshipDecayModify.Framework;
using weizinai.StardewValleyMod.FriendshipDecayModify.Patcher;
using weizinai.StardewValleyMod.PiCore.Extension;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.FriendshipDecayModify;

internal class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger<ModEntry>.Init(this);
        this.config = helper.ReadConfig<ModConfig>();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(
            this,
            new GameLocationPatcher(this.config),
            new FarmerPatcher(this.config),
            new NPCPatcher(this.config),
            new FarmAnimalPatcher(this.config)
        );
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            () => this.config,
            value => this.config = value,
            configMenu => configMenu
                // 每日对话修改
                .AddSectionTitle(I18n.Config_DailyGreetingModifyTitle_Name)
                .AddNumberOption(
                    config => config.DailyGreetingModifyForVillager,
                    (config, value) => config.DailyGreetingModifyForVillager = value,
                    I18n.Config_DailyGreetingModifyForVillager_Name
                )
                .AddNumberOption(
                    config => config.DailyGreetingModifyForDatingVillager,
                    (config, value) => config.DailyGreetingModifyForDatingVillager = value,
                    I18n.Config_DailyGreetingModifyForDatingVillager_Name
                )
                .AddNumberOption(
                    config => config.DailyGreetingModifyForSpouse,
                    (config, value) => config.DailyGreetingModifyForSpouse = value,
                    I18n.Config_DailyGreetingModifyForSpouse_Name
                )
                // 礼物修改
                .AddSectionTitle(I18n.Config_GiftModifyTitle_Name)
                .AddNumberOption(
                    config => config.DislikeGiftModify,
                    (config, value) => config.DislikeGiftModify = value,
                    I18n.Config_DislikeGiftModify_Name
                )
                .AddNumberOption(
                    config => config.HateGiftModify,
                    (config, value) => config.HateGiftModify = value,
                    I18n.Config_HateGiftModify_Name
                )
                // 垃圾桶修改
                .AddSectionTitle(I18n.Config_GarbageCanModify_Name)
                .AddNumberOption(
                    config => config.GarbageCanModify,
                    (config, value) => config.GarbageCanModify = value,
                    I18n.Config_GarbageCanModify_Name
                )
                // 动物好感度修改
                .AddSectionTitle(I18n.Config_AnimalFriendshipModifyTitle_Name)
                .AddNumberOption(
                    config => config.PetAnimalModifyForFriendship,
                    (config, value) => config.PetAnimalModifyForFriendship = value,
                    I18n.Config_PetAnimalModifyForFriendship_Name
                )
                .AddNumberOption(
                    config => config.FeedAnimalModifyForFriendship,
                    (config, value) => config.FeedAnimalModifyForFriendship = value,
                    I18n.Config_FeedAnimalModifyForFriendship_Name
                )
                // 动物心情修改
                .AddSectionTitle(I18n.Config_AnimalHappinessModifyTitle_Name)
                .AddNumberOption(
                    config => config.PetAnimalModifyForHappiness,
                    (config, value) => config.PetAnimalModifyForHappiness = value,
                    I18n.Config_PetAnimalModifyForHappiness_Name
                )
                .AddNumberOption(
                    config => config.FeedAnimalModifyForHappiness,
                    (config, value) => config.FeedAnimalModifyForHappiness = value,
                    I18n.Config_FeedAnimalModifyForHappiness_Name
                )
        );
    }
}
