using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Quests;
using SObject = StardewValley.Object;

namespace HelpWanted;

internal partial class ModEntry
{
    /// <summary>获取自定义Pin纹理,如果没有则返回默认纹理</summary>
    public static Texture2D GetPinTexture(string target, string questType)
    {
        // 获取特定NPC和特定任务类型的任务的自定义Pin纹理
        var texture = GetTexture(PinTexturePath + "/" + target + "/" + questType);
        if (texture != null)
        {
            return texture;
        }

        // 获取特定NPC的任务的自定义Pin纹理
        texture = GetTexture(PinTexturePath + "/" + target);
        if (texture is not null)
        {
            return texture;
        }

        // 获取特定任务类型的任务的自定义Pin纹理
        texture = GetTexture(PinTexturePath + "/" + questType);
        if (texture is not null)
        {
            return texture;
        }

        // 获取自定义Pin纹理
        texture = GetTexture(PinTexturePath);
        if (texture is not null)
        {
            return texture;
        }

        // 如果没有自定义纹理,则返回默认Pin纹理
        return SHelper.ModContent.Load<Texture2D>("Assets/Pin.png");
    }

    /// <summary>获取自定义Pad纹理,如果没有则返回默认纹理</summary>
    public static Texture2D GetPadTexture(string target, string questType)
    {
        // 获取特定NPC和特定任务类型的任务的自定义Pad纹理
        var texture = GetTexture(PadTexturePath + "/" + target + "/" + questType);
        if (texture is not null)
        {
            return texture;
        }

        // 获取特定NPC的任务的自定义Pad纹理
        texture = GetTexture(PadTexturePath + "/" + target);
        if (texture is not null)
        {
            return texture;
        }

        // 获取特定任务类型的任务的自定义Pad纹理
        texture = GetTexture(PadTexturePath + "/" + questType);
        if (texture is not null)
        {
            return texture;
        }

        // 获取自定义Pad纹理
        texture = GetTexture(PadTexturePath);
        if (texture is not null)
        {
            return texture;
        }

        // 如果没有自定义纹理,则返回默认Pad纹理
        return SHelper.ModContent.Load<Texture2D>("Assets/Pad.png");
    }

    /// <summary>获取自定义纹理</summary>
    private static Texture2D? GetTexture(string path)
    {
        // 获取特定NPC或特定任务类型的任务的不同自定义纹理,如果纹理存在,则随机返回一个
        var textures = new List<Texture2D>();
        try
        {
            for (var i = 1;; i++)
                textures.Add(SHelper.GameContent.Load<Texture2D>(path + "/" + i));
        }
        catch
        {
            // ignored
        }
        if (textures.Any())
        {
            return textures[Game1.random.Next(textures.Count)];
        }

        // 获取特定NPC或特定任务类型的任务的自定义纹理
        try
        {
            return SHelper.GameContent.Load<Texture2D>(path);
        }
        catch
        {
            // ignored
        }

        return null;
    }

    /// <summary>获取随机颜色</summary>
    public static Color GetRandomColor()
    {
        return new Color((byte)Random.Next(Config.RandomColorMin, Config.RandomColorMax),
            (byte)Random.Next(Config.RandomColorMin, Config.RandomColorMax),
            (byte)Random.Next(Config.RandomColorMin, Config.RandomColorMax));
    }

    /// <summary>获取随机物品</summary>
    public static string GetRandomItem(string result, List<string>? possibleItems)
    {
        // 获取允许的任务物品列表
        var items = GetRandomItemList(possibleItems);

        // 如果物品列表为空或者物品列表包含原随机结果,则返回原随机结果
        if (items is null || items.Contains(result))
            return result;

        // 遍历物品列表,去掉其中不符合条件的物品
        items = items.Where(item => Game1.objectData.ContainsKey(item)).ToList();
        // 如果物品列表为空,则返回原随机结果,否则返回随机物品
        return !items.Any() ? result : items[Random.Next(items.Count)];
    }

    /// <summary>获取随机物品列表</summary>
    private static List<string>? GetRandomItemList(List<string>? possibleItems)
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

        // 如果物品字符串为空,则返回null
        if (string.IsNullOrEmpty(itemString))
            return null;

        // 遍历所有物品,移除不符合条件的物品
        var items = new List<string>(itemString.Split(' '));
        items = items.Where(item =>
            {
                var sObject = new SObject(item, 1);
                return !(!Config.AllowArtisanGoods && sObject.Category == SObject.artisanGoodsCategory) &&
                       !(Config.MaxPrice > 0 && sObject.Price > Config.MaxPrice);
            }
        ).ToList();

        // 如果物品列表为空,则返回null
        if (!items.Any()) return null;

        // 如果物品列表不为空,则根据是否忽略原版物品限制执行不同操作
        switch (Config.IgnoreVanillaItemRestriction)
        {
            // 如果忽略原版物品限制,则返回该物品列表
            case true:
                return items;
            // 如果不忽略原版物品限制,且可能的物品列表不为空,则对该列表进行筛选        
            case false when possibleItems?.Any() != null:
                possibleItems = possibleItems.Where(item =>
                {
                    var sObject = new SObject(item, 1);
                    return items.Contains(item) &&
                           items.Contains(sObject.Category.ToString()) &&
                           !(!Config.AllowArtisanGoods && sObject.Category == SObject.artisanGoodsCategory) &&
                           !(Config.MaxPrice > 0 && sObject.Price > Config.MaxPrice);
                }).ToList();
                if (possibleItems.Any())
                {
                    return possibleItems;
                }

                break;
        }

        return null;
    }

    public static List<string> GetPossibleCrops(List<string> oldList)
    {
        if (!Config.ModEnabled)
            return oldList;
        var newList = GetRandomItemList(oldList);
        return newList is null || !newList.Any() ? oldList : newList;
    }
}