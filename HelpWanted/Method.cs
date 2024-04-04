using HelpWanted.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Quests;

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

    public static Quest MakeQuest(QuestInfo quest)
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

    private static void AddQuest(Quest quest, QuestType questType, Texture2D icon, Rectangle iconRect, Point iconOffset)
    {
        var npcName = SHelper.Reflection.GetField<NetString>(quest, "target").GetValue().Value;
        var padTexture = GetPadTexture(npcName, questType.ToString());
        var pinTexture = GetPinTexture(npcName, questType.ToString());
        questList.Add(new QuestData(padTexture, pinTexture, icon, iconRect, iconOffset));
    }

    /*private static int GetRandomItem(int result, List<int> possibleItems)
    {
        List<int> items = GetRandomItemList(possibleItems);

        if (items is null)
            return result;
        if (items.Contains(result) && !Config.IgnoreVanillaItemSelection)
            return result;
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (!Game1.objectInformation.ContainsKey(items[i]))
                items.RemoveAt(i);
        }

        if (!items.Any())
            return result;
        var ii = items[random.Next(items.Count)];
        if (!Game1.objectInformation.ContainsKey(ii))
            return result;
        //SMonitor.Log($"our random: {ii}");
        //SMonitor.Log($"found our random: {ii}");
        return ii;
    }

    private static List<int> GetRandomItemList(List<int> possibleItems)
    {
        if (!Config.ModEnabled || Config is { MustLikeItem: false, MustLoveItem: false } || Game1.questOfTheDay is not ItemDeliveryQuest)
            return null;
        string name = (Game1.questOfTheDay as ItemDeliveryQuest)?.target?.Value;
        if (name is null || !Game1.NPCGiftTastes.TryGetValue(name, out string data))
            return null;
        var listString = Game1.NPCGiftTastes["Universal_Love"];
        if (!Config.MustLoveItem)
        {
            listString += " " + Game1.NPCGiftTastes["Universal_Like"];
        }

        var split = data.Split('/');
        if (split.Length < 4)
            return null;
        listString += " " + split[1] + (Config.MustLoveItem ? "" : " " + split[3]);
        if (string.IsNullOrEmpty(listString))
            return null;
        split = listString.Split(' ');
        List<int> items = new List<int>();
        foreach (var str in split)
        {
            if (!int.TryParse(str, out int i) || !Game1.objectInformation.ContainsKey(i))
                continue;
            SObject obj = new SObject(i, 1);
            if (!Config.AllowArtisanGoods && obj is not null && obj.Category == SObject.artisanGoodsCategory)
                continue;
            if (Config.MaxPrice > 0 && obj is not null && obj.Price > Config.MaxPrice)
                continue;
            items.Add(i);
        }

        if (!items.Any() || (!Config.IgnoreVanillaItemSelection && possibleItems?.Any() != true))
            return null;
        if (Config.IgnoreVanillaItemSelection)
        {
            return items;
        }

        for (int i = possibleItems.Count - 1; i >= 0; i--)
        {
            int idx = possibleItems[i];
            if (!items.Contains(idx))
            {
                if (idx >= 0)
                {
                    SObject obj = new SObject(idx, 1);
                    if (obj is null || !items.Contains(obj.Category) ||
                        (!Config.AllowArtisanGoods && obj.Category == SObject.artisanGoodsCategory) ||
                        (Config.MaxPrice > 0 && obj.Price > Config.MaxPrice))
                    {
                        possibleItems.RemoveAt(i);
                    }
                }
            }
        }

        if (possibleItems.Any())
        {
            return possibleItems;
        }
        else
        {
            return items;
        }
    }*/
}