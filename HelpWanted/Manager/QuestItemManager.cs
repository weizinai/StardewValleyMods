using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Extensions;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Helper;

namespace weizinai.StardewValleyMod.HelpWanted.Manager;

public class QuestItemManager
{
    public static QuestItemManager Instance { get; } = new();

    private VanillaModConfig Config => ModConfig.Instance.VanillaConfig;

    private readonly List<string> universalGiftTaste = new();
    private readonly Dictionary<string, List<string>> possibleItems = new();
    private readonly Dictionary<string, List<string>> possibleCrops = new();

    public string GetRandomItem(string npcName)
    {
        if (!this.possibleItems.ContainsKey(npcName))
        {
            this.InitPossibleItems(npcName);
        }

        if (this.possibleItems[npcName].Any())
        {
            return ModEntry.Random.ChooseFrom(this.possibleItems[npcName]);
        }

        Logger.Info($"No qualifying items found in {npcName}'s gift taste. Generating a random item through vanilla logic.");
        return Utility.getRandomItemFromSeason(Game1.season, true, ModEntry.Random);
    }

    public string GetRandomCrop(string npcName)
    {
        if (!this.possibleCrops.ContainsKey(npcName))
        {
            this.InitPossibleCrops(npcName);
        }

        if (this.possibleCrops[npcName].Any())
        {
            return ModEntry.Random.ChooseFrom(this.possibleCrops[npcName]);
        }

        Logger.Info($"No qualifying crops found in {npcName}'s gift taste. Generating a random crop through vanilla logic.");
        return ModEntry.Random.ChooseFrom(Utility.possibleCropsAtThisTime(Game1.season, Game1.dayOfMonth <= 7));
    }

    public void ClearCache()
    {
        this.universalGiftTaste.Clear();
        this.possibleItems.Clear();
        this.possibleCrops.Clear();
    }

    private void InitPossibleItems(string npcName)
    {
        this.possibleItems[npcName] = new List<string>();

        this.LoadUniversalGiftTaste();
        foreach (var itemId in this.universalGiftTaste)
        {
            if (CheckItemId(itemId))
            {
                this.possibleItems[npcName].Add(itemId);
            }
        }

        foreach (var itemId in this.GetNPCGiftTaste(npcName))
        {
            if (CheckItemId(itemId))
            {
                this.possibleItems[npcName].Add(itemId);
            }
        }

        this.possibleItems[npcName].RemoveAll(itemId =>
        {
            var obj = new SObject(itemId, 1);
            return this.Config.MaxPrice >= 0 && obj.Price >= this.Config.MaxPrice
                   || !this.Config.AllowArtisanGoods && obj.Category == SObject.artisanGoodsCategory;
        });

        return;

        bool CheckItemId(string itemId)
        {
            return itemId == "-5" || itemId == "-6" || ItemRegistry.Exists(itemId) && !CropChecker.IsCrop(itemId);
        }
    }

    private void InitPossibleCrops(string npcName)
    {
        this.possibleCrops[npcName] = new List<string>();

        this.LoadUniversalGiftTaste();
        foreach (var itemId in this.universalGiftTaste)
        {
            if (CheckItemId(itemId))
            {
                this.possibleCrops[npcName].Add(itemId);
            }
        }

        foreach (var itemId in this.GetNPCGiftTaste(npcName))
        {
            if (CheckItemId(itemId))
            {
                this.possibleCrops[npcName].Add(itemId);
            }
        }

        this.possibleCrops[npcName].RemoveAll(itemId =>
        {
            var obj = new SObject(itemId, 1);
            return this.Config.MaxPrice >= 0 && obj.Price >= this.Config.MaxPrice;
        });

        return;

        bool CheckItemId(string itemId)
        {
            return CropChecker.IsCrop(itemId) && CropChecker.IsCurrentSeasonCrop(itemId);
        }
    }

    private void LoadUniversalGiftTaste()
    {
        if (this.universalGiftTaste.Any()) return;

        this.universalGiftTaste.AddRange(ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Love"]));
        if (this.Config.QuestItemRequirement > 0) this.universalGiftTaste.AddRange(ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Like"]));
        if (this.Config.QuestItemRequirement > 1) this.universalGiftTaste.AddRange(ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Neutral"]));
        if (this.Config.QuestItemRequirement > 2) this.universalGiftTaste.AddRange(ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Dislike"]));
        if (this.Config.QuestItemRequirement > 3) this.universalGiftTaste.AddRange(ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Hate"]));
    }

    private List<string> GetNPCGiftTaste(string npcName)
    {
        var giftTaste = new List<string>();

        if (!Game1.NPCGiftTastes.TryGetValue(npcName, out var rawData))
        {
            Logger.Error($"Failed to retrieve the gift taste for {npcName} in step 1.");
            return new List<string>();
        }

        var data = rawData.Split('/');
        if (data.Length < 10)
        {
            Logger.Error($"Failed to retrieve the gift taste for {npcName} in step 2.");
            return new List<string>();
        }

        giftTaste.AddRange(ArgUtility.SplitBySpace(data[1]));
        if (this.Config.QuestItemRequirement > 0) giftTaste.AddRange(ArgUtility.SplitBySpace(data[3]));
        if (this.Config.QuestItemRequirement > 1) giftTaste.AddRange(ArgUtility.SplitBySpace(data[5]));
        if (this.Config.QuestItemRequirement > 2) giftTaste.AddRange(ArgUtility.SplitBySpace(data[7]));
        if (this.Config.QuestItemRequirement > 3) giftTaste.AddRange(ArgUtility.SplitBySpace(data[9]));

        return giftTaste;
    }
}