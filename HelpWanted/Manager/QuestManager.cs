using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.Config;
using weizinai.StardewValleyMod.HelpWanted.Helper;
using weizinai.StardewValleyMod.HelpWanted.Model;
using weizinai.StardewValleyMod.HelpWanted.UI;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.HelpWanted.Manager;

/// <summary>
/// 一块任务板（原版 / RSV）当天的任务与待接便签：任务生成写进 <see cref="QuestList"/>，
/// 上板时由管理器完成摆放并转成 <see cref="PendingNotes"/>，界面只向下读这些便签。
/// </summary>
/// <typeparam name="T">管理器自身的类型，用于让每个管理器各拿到自己的单例。</typeparam>
public abstract class QuestManager<T> : IQuestManager where T : class, new()
{
    // 摆放专用的随机源：与 ModEntry.Random（任务生成）分开，避免两处共用实例而互相牵动取值顺序
    private readonly Random placementRandom = new();

    private readonly List<QuestNote> pendingNotes = new();

    public static T Instance { get; } = new();

    protected readonly List<QuestModel> QuestList = new();

    /// <inheritdoc/>
    public IReadOnlyList<QuestNote> PendingNotes => this.pendingNotes;

    /// <inheritdoc/>
    public bool HasPendingQuests => this.QuestList.Count > 0 || this.pendingNotes.Count > 0;

    /// <inheritdoc/>
    public void PlacePendingNotes(Rectangle boardBounds)
    {
        if (this.QuestList.Count <= 0) return;

        // 摆放顺序沿用旧实现：从任务列表尾部倒序摆放
        var questsToPlace = this.QuestList.ToList();
        questsToPlace.Reverse();

        var noteSizes = questsToPlace.Select(quest => new Point((int)quest.NoteWidth, (int)quest.NoteHeight)).ToList();
        var occupiedBounds = this.pendingNotes.Select(note => note.PlacedBounds).ToList();
        var overlapBoundary = new NotePlacement.Boundary(ModConfig.Instance.XOverlapBoundary, ModConfig.Instance.YOverlapBoundary);
        var placement = NotePlacement.PlaceAll(boardBounds, noteSizes, overlapBoundary, occupiedBounds, this.placementRandom);

        for (var i = 0; i < questsToPlace.Count; i++)
        {
            this.pendingNotes.Add(new QuestNote(questsToPlace[i], placement.Bounds[i]));
        }

        this.QuestList.Clear();

        if (placement.Relaxed)
        {
            Logger<ModEntry>.Debug($"Relaxed note overlap boundaries to fit all {questsToPlace.Count} quests on the board.");
        }
    }

    /// <inheritdoc/>
    public void RemovePendingNote(QuestNote note)
    {
        ArgumentNullException.ThrowIfNull(note);
        this.pendingNotes.Remove(note);
    }

    /// <summary>清空当天的任务与待接便签；每日开始时调用，避免上一日的数据残留到新的一天。</summary>
    public void ClearCache()
    {
        this.QuestList.Clear();
        this.pendingNotes.Clear();
    }

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

    protected QuestModel GetQuestData(NPC npc, Quest quest)
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

        return new QuestModel(
            padTexture, padTextureSource, padColor,
            pinTexture, pinTextureSource, pinColor,
            icon, iconSource, iconColor,
            quest
        );
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
}
