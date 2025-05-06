using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Model;

namespace weizinai.StardewValleyMod.HelpWanted.Manager;

public abstract class QuestManager<T> where T : class, new()
{
    public static T Instance { get; } = new();
    public readonly List<QuestData> QuestList = new();

    protected NPC? GetNPCFromQuest(Quest quest)
    {
        return quest switch
        {
            ItemDeliveryQuest itemDeliveryQuest => Game1.getCharacterFromName(itemDeliveryQuest.target.Value),
            ResourceCollectionQuest resourceCollectionQuest => Game1.getCharacterFromName(resourceCollectionQuest.target.Value),
            SlayMonsterQuest slayMonsterQuest => Game1.getCharacterFromName(slayMonsterQuest.target.Value),
            FishingQuest fishingQuest => Game1.getCharacterFromName(fishingQuest.target.Value),
            LostItemQuest lostItemQuest => Game1.getCharacterFromName(lostItemQuest.npcName.Value),
            _ => null
        };
    }

    protected QuestData GetQuestData(NPC npc, Quest quest)
    {
        var questType = this.GetQuestType(quest);
        var padTexture = TextureManager.Instance.GetPadTexture(npc.Name, questType.ToString());
        var padTextureSource = new Rectangle(0, 0, 64, 64);
        var padColor = TextureManager.Instance.GetRandomColor();
        var pinTexture = TextureManager.Instance.GetPinTexture(npc.Name, questType.ToString());
        var pinTextureSource = new Rectangle(0, 0, 64, 64);
        var pinColor = TextureManager.Instance.GetRandomColor();
        var icon = npc.Portrait;
        var iconColor = new Color(
            ModConfig.Instance.PortraitTintR,
            ModConfig.Instance.PortraitTintG,
            ModConfig.Instance.PortraitTintB,
            ModConfig.Instance.PortraitTintA
        );
        var iconSource = new Rectangle(0, 0, 64, 64);
        var iconScale = ModConfig.Instance.PortraitScale;
        var iconOffset = new Point(ModConfig.Instance.PortraitOffsetX, ModConfig.Instance.PortraitOffsetY);
        return new QuestData(padTexture, padTextureSource, padColor, pinTexture, pinTextureSource, pinColor,
            icon, iconSource, iconColor, iconScale, iconOffset, quest);
    }

    protected QuestType GetQuestType(Quest quest)
    {
        return quest switch
        {
            ItemDeliveryQuest => QuestType.ItemDelivery,
            ResourceCollectionQuest => QuestType.ResourceCollection,
            SlayMonsterQuest => QuestType.SlayMonster,
            FishingQuest => QuestType.Fishing,
            LostItemQuest => QuestType.LostItem,
            _ => QuestType.Unknown
        };
    }

    public void ClearCache()
    {
        this.QuestList.Clear();
    }
}