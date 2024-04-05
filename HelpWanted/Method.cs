using HelpWanted.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Quests;
using SObject = StardewValley.Object;

namespace HelpWanted;

internal partial class ModEntry
{
    public static Texture2D GetPinTexture(string target, string questType)
    {
        var texture = GetTexture(PinTexturePath + "/" + target + "/" + questType);
        if (texture != null)
        {
            return texture;
        }

        texture = GetTexture(PinTexturePath + "/" + target);
        if (texture is not null)
        {
            return texture;
        }

        texture = GetTexture(PinTexturePath + "/" + questType);
        if (texture is not null)
        {
            return texture;
        }

        texture = GetTexture(PinTexturePath);
        if (texture is not null)
        {
            return texture;
        }

        return SHelper.ModContent.Load<Texture2D>("Assets/Pin.png");
    }

    public static Texture2D GetPadTexture(string target, string questType)
    {
        var texture = GetTexture(PadTexturePath + "/" + target + "/" + questType);
        if (texture is not null)
        {
            return texture;
        }

        texture = GetTexture(PadTexturePath + "/" + target);
        if (texture is not null)
        {
            return texture;
        }

        texture = GetTexture(PadTexturePath + "/" + questType);
        if (texture is not null)
        {
            return texture;
        }

        texture = GetTexture(PadTexturePath);
        if (texture is not null)
        {
            return texture;
        }

        return SHelper.ModContent.Load<Texture2D>("Assets/Pad.png");
    }

    private static Texture2D? GetTexture(string path)
    {
        var list = new List<Texture2D>();
        try
        {
            for (var i = 1;; i++)
                list.Add(SHelper.GameContent.Load<Texture2D>(path + "/" + i));
        }
        catch
        {
        }

        if (list.Any())
        {
            return list[Game1.random.Next(list.Count)];
        }

        try
        {
            return SHelper.GameContent.Load<Texture2D>(path);
        }
        catch
        {
        }

        return null;
    }

    public static Color GetRandomColor()
    {
        return new Color((byte)Random.Next(Config.RandomColorMin, Config.RandomColorMax),
            (byte)Random.Next(Config.RandomColorMin, Config.RandomColorMax),
            (byte)Random.Next(Config.RandomColorMin, Config.RandomColorMax));
    }

    public static Quest CreateQuest(QuestInfo quest)
    {
        switch (quest.QuestType)
        {
            case QuestType.ResourceCollection:
                var q = new ResourceCollectionQuest
                {
                    questTitle = quest.QuestTitle,
                    questDescription = quest.QuestDescription,
                    ItemId =
                    {
                        Value = quest.ItemId
                    },
                    number =
                    {
                        Value = quest.Number
                    },
                    target =
                    {
                        Value = quest.Target
                    },
                    targetMessage =
                    {
                        Value = quest.TargetMessage
                    },
                    currentObjective = quest.CurrentObjective
                };
                return q;
            case QuestType.Fishing:
                var q2 = new FishingQuest
                {
                    questTitle = quest.QuestTitle,
                    questDescription = quest.QuestDescription,
                    ItemId =
                    {
                        Value = quest.ItemId
                    },
                    numberToFish =
                    {
                        Value = quest.Number
                    },
                    target =
                    {
                        Value = quest.Target
                    },
                    targetMessage = quest.TargetMessage,
                    currentObjective = quest.CurrentObjective
                };
                return q2;
            case QuestType.ItemDelivery:
                var q3 = new ItemDeliveryQuest
                {
                    ItemId =
                    {
                        Value = quest.ItemId
                    },
                    questTitle = quest.QuestTitle,
                    questDescription = quest.QuestDescription,
                    target =
                    {
                        Value = quest.Target
                    },
                    targetMessage = quest.TargetMessage,
                    currentObjective = quest.CurrentObjective
                };
                return q3;
            case QuestType.SlayMonster:
                var q4 = new SlayMonsterQuest
                {
                    questTitle = quest.QuestTitle,
                    questDescription = quest.QuestDescription,
                    monsterName =
                    {
                        Value = quest.ItemId
                    },
                    numberToKill =
                    {
                        Value = quest.Number
                    },
                    target =
                    {
                        Value = quest.Target
                    },
                    targetMessage = quest.TargetMessage,
                    currentObjective = quest.CurrentObjective
                };
                return q4;
            default:
                return null;
        }
    }

    /// <summary>刷新每日任务</summary>
    private static void RefreshQuestOfTheDay()
    {
        // 玩家在矿井中达到的最大层数大于0并且游戏天数大于5天.则可以接到杀怪任务
        var mine = (MineShaft.lowestLevelReached > 0 && Game1.stats.DaysPlayed > 5U);
        // 总权重
        var totalWeight = Config.ResourceCollectionWeight + (mine ? Config.SlayMonstersWeight : 0) + Config.FishingWeight +
                          Config.ItemDeliveryWeight;
        // 生成一个0-1之间的随机双浮点数
        var randomDouble = Random.NextDouble();
        double currentWeight = 0;

        var questTypes = new List<(double weight, Func<Quest> createQuest)>
        {
            (Config.ResourceCollectionWeight, () => new ResourceCollectionQuest()),
            (mine ? Config.SlayMonstersWeight : 0, () => new SlayMonsterQuest()),
            (Config.FishingWeight, () => new FishingQuest()),
            (Config.ItemDeliveryWeight, () => new ItemDeliveryQuest())
        };

        foreach (var (weight, createQuest) in questTypes)
        {
            currentWeight += weight;
            if (randomDouble < currentWeight / totalWeight)
            {
                Game1.netWorldState.Value.SetQuestOfTheDay(createQuest());
                return;
            }
        }
    }

    private static void AddQuest(Quest quest, QuestType questType, Texture2D icon)
    {
        var npcName = SHelper.Reflection.GetField<NetString>(quest, "target").GetValue().Value;
        var padTexture = GetPadTexture(npcName, questType.ToString());
        var pinTexture = GetPinTexture(npcName, questType.ToString());
        QuestList.Add(new QuestData(padTexture, pinTexture, icon));
    }

    private static string GetRandomItem(string result, List<string> possibleItems)
    {
        var items = GetRandomItemList(possibleItems);
        
        if (items is null)
            return ItemRegistry.QualifyItemId(result);
        if (items.Contains(result) && !Config.IgnoreVanillaItemSelection)
            return ItemRegistry.QualifyItemId(result);;
        
        for (var i = items.Count - 1; i >= 0; i--)
        {
            if (!Game1.objectData.ContainsKey(items[i]))
                items.RemoveAt(i);
        }

        if (!items.Any())
            return result;
        
        var ii = items[Random.Next(items.Count)];
        if (!Game1.objectData.ContainsKey(ii))
            return result;
        //SMonitor.Log($"our random: {ii}");
        //SMonitor.Log($"found our random: {ii}");
        return ii;
    }

    private static List<string>? GetRandomItemList(List<string> possibleItems)
    {
        // 如果模组未启用,或者必须为喜爱物品和必须为喜欢物品均为false,或者今日任务不是物品交付任务,则返回null
        if (!Config.ModEnabled || Config is { MustLikeItem: false, MustLoveItem: false } ||
            Game1.questOfTheDay is not ItemDeliveryQuest itemDeliveryQuest)
            return null;

        // 获取普遍喜爱物品和普遍喜欢物品
        var itemString = Game1.NPCGiftTastes["Universal_Love"];
        if (!Config.MustLoveItem)
        {
            itemString += " " + Game1.NPCGiftTastes["Universal_Like"];
        }

        // 获取任务目标NPC的喜爱物品和喜欢物品
        var npcName = itemDeliveryQuest.target.Value;
        if (npcName is null || !Game1.NPCGiftTastes.TryGetValue(npcName, out var data))
            return null;
        var split = data.Split('/');
        if (split.Length < 4) return null;
        itemString += " " + split[1] + (Config.MustLoveItem ? "" : " " + split[3]);
        if (string.IsNullOrEmpty(itemString))
            return null;
        var items = new List<string>(itemString.Split(' '));
        
        // 遍历所有物品,移除不符合条件的物品
        foreach (var item in items)
        {
            var sObject = new SObject(item, 1);
            if (!Config.AllowArtisanGoods && sObject.Category == SObject.artisanGoodsCategory)
            {
                items.Remove(item);
                continue;
            }

            if (Config.MaxPrice > 0 && sObject.Price > Config.MaxPrice)
            {
                items.Remove(item);
            }
        }

        if (!items.Any() || (!Config.IgnoreVanillaItemSelection && possibleItems.Any() != true))
            return null;

        if (Config.IgnoreVanillaItemSelection)
        {
            return items;
        }

        foreach (var item in possibleItems)
        {
            if (!items.Contains(item))
            {
                var sObject = new SObject(item, 1);
                if (!items.Contains(sObject.Category.ToString()) ||
                    (!Config.AllowArtisanGoods && sObject.Category == SObject.artisanGoodsCategory) ||
                    (Config.MaxPrice > 0 && sObject.Price > Config.MaxPrice))
                {
                    possibleItems.Remove(item);
                }
            }
        }

        if (possibleItems.Any())
        {
            return possibleItems;
        }

        return items;
    }
    
    private static List<string> GetPossibleCrops(List<string> oldList)
    {
        if (!Config.ModEnabled)
            return oldList;
        List<string> newList = GetRandomItemList(oldList);
        //SMonitor.Log($"possible crops: {newList?.Count}");
        return (newList is null || !newList.Any()) ? oldList : newList;
    }
}