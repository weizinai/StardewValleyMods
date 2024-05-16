using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace FriendshipDecayModify.Framework;

internal class ModConfig
{
    public KeybindList OpenConfigMenu { get; set; } = new(SButton.None);

    // 每日对话修改
    public int DailyGreetingModifyForVillager { get; set; } = 2;
    public int DailyGreetingModifyForDatingVillager { get; set; } = 8;
    public int DailyGreetingModifyForSpouse { get; set; } = 20;

    // 礼物修改
    public int DislikeGiftModify { get; set; } = 20;
    public int HateGiftModify { get; set; } = 40;

    // 垃圾桶修改
    public int GarbageCanModify { get; set; } = 25;

    // 动物友谊修改
    public int PetAnimalModifyForFriendship { get; set; } = 10;
    public int FeedAnimalModifyForFriendship { get; set; } = 20;

    // 动物心情修改
    public int PetAnimalModifyForHappiness { get; set; } = 50;
    public int FeedAnimalModifyForHappiness { get; set; } = 100;
}