using StardewModdingAPI;
using weizinai.StardewValleyMod.FriendshipDecayModify.Framework;
using weizinai.StardewValleyMod.FriendshipDecayModify.Patcher;
using weizinai.StardewValleyMod.PiCore.Config;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.FriendshipDecayModify;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        var configService = new ConfigService<ModConfig>(this);
        configService.RegisterMenu(this.BuildConfigMenu);
        // 注册Harmony补丁
        HarmonyPatcher.Apply(
            this,
            new GameLocationPatcher(),
            new FarmerPatcher(),
            new NPCPatcher(),
            new FarmAnimalPatcher()
        );
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            // 每日对话修改
            .AddSectionTitle(I18n.Config_DailyGreetingModifyTitle_Name)
            .AddNumberOption(config => config.DailyGreetingModifyForVillager, I18n.Config_DailyGreetingModifyForVillager_Name)
            .AddNumberOption(config => config.DailyGreetingModifyForDatingVillager, I18n.Config_DailyGreetingModifyForDatingVillager_Name)
            .AddNumberOption(config => config.DailyGreetingModifyForSpouse, I18n.Config_DailyGreetingModifyForSpouse_Name)
            // 礼物修改
            .AddSectionTitle(I18n.Config_GiftModifyTitle_Name)
            .AddNumberOption(config => config.DislikeGiftModify, I18n.Config_DislikeGiftModify_Name)
            .AddNumberOption(config => config.HateGiftModify, I18n.Config_HateGiftModify_Name)
            // 垃圾桶修改
            .AddSectionTitle(I18n.Config_GarbageCanModify_Name)
            .AddNumberOption(config => config.GarbageCanModify, I18n.Config_GarbageCanModify_Name)
            // 动物好感度修改
            .AddSectionTitle(I18n.Config_AnimalFriendshipModifyTitle_Name)
            .AddNumberOption(config => config.PetAnimalModifyForFriendship, I18n.Config_PetAnimalModifyForFriendship_Name)
            .AddNumberOption(config => config.FeedAnimalModifyForFriendship, I18n.Config_FeedAnimalModifyForFriendship_Name)
            // 动物心情修改
            .AddSectionTitle(I18n.Config_AnimalHappinessModifyTitle_Name)
            .AddNumberOption(config => config.PetAnimalModifyForHappiness, I18n.Config_PetAnimalModifyForHappiness_Name)
            .AddNumberOption(config => config.FeedAnimalModifyForHappiness, I18n.Config_FeedAnimalModifyForHappiness_Name);
    }
}
