using System.Text.Json;
using StardewValley;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

internal static class SpendLimitHelper
{
    private const int DefaultErrorLimit = 1234;

    public static bool TryGetFarmerSpendLimit(string name, out int limit)
    {
        Game1.MasterPlayer.modData.TryGetValue(SpendLimitHandler.SpendLimitDataKey, out var rawLimitData);

        if (rawLimitData == null)
        {
            Log.Error($"主机端未开启花钱限制功能，但却尝试获取{name}的消费额度");
            limit = DefaultErrorLimit;
            return false;
        }

        var limitData = JsonSerializer.Deserialize<Dictionary<string, int>>(rawLimitData)!;
        if (limitData.TryGetValue(name, out var value))
        {
            limit = value;
            return true;
        }

        Log.Error($"无法获取{name}的消费额度，其额度已自动设置为{DefaultErrorLimit}");
        limit = DefaultErrorLimit;
        return false;
    }

    public static void GetFarmerSpendData(out int amount, out int limit, out int availableMoney)
    {
        var player = Game1.player;
        TryGetFarmerSpendLimit(player.Name, out limit);
        amount = int.Parse(player.modData[SpendLimitHandler.SpendAmountKey]);
        availableMoney = limit - amount;
    }

    public static void SetFarmerSpendLimit(string name, int limit)
    {
        var modData = Game1.MasterPlayer.modData;

        modData.TryGetValue(SpendLimitHandler.SpendLimitDataKey, out var rawLimitData);

        if (rawLimitData == null)
        {
            Log.Error($"主机端未开启花钱限制功能，但却尝试设置{name}的消费额度");
            return;
        }

        var limitData = JsonSerializer.Deserialize<Dictionary<string, int>>(rawLimitData)!;
        limitData[name] = limit;
        modData[SpendLimitHandler.SpendLimitDataKey] = JsonSerializer.Serialize(limitData);
    }

    public static bool IsSpendLimitEnable()
    {
        return Game1.IsClient && Game1.MasterPlayer.modData.ContainsKey(SpendLimitHandler.SpendLimitKey);
    }
}