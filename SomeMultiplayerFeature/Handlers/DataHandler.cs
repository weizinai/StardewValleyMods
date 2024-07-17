using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Machines;
using StardewValley.GameData.Shops;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class DataHandler : BaseHandler
{
    public DataHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
    }

    public override void Clear()
    {
        this.Helper.Events.Content.AssetRequested -= this.OnAssetRequested;
    }

    // 修改收获机器经验
    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo("Data/Machines"))
        {
            e.Edit(asset =>
                {
                    var machineData = asset.AsDictionary<string, MachineData>().Data;
                    machineData["(BC)12"].ExperienceGainOnHarvest = "farming 20";          // 小桶
                    machineData["(BC)13"].ExperienceGainOnHarvest = "mining 7";            // 熔炉
                    machineData["(BC)20"].ExperienceGainOnHarvest = "fishing 2";           // 回收机
                    machineData["(BC)25"].ExperienceGainOnHarvest = "farming 4";           // 种子生成器
                    machineData["(BC)105"].ExperienceGainOnHarvest = "foraging 4";         // 树液采集器
                    machineData["(BC)114"].ExperienceGainOnHarvest = "foraging 4";         // 煤炭窑
                    machineData["(BC)FishSmoker"].ExperienceGainOnHarvest = "fishing 2";   // 熏鱼机
                    machineData["(BC)HeavyFurnace"].ExperienceGainOnHarvest = "mining 35"; // 重型熔炉
                }
            );
            Log.Info("添加收获机器经验：\n小桶：20点耕种经验\n熔炉：7点采矿经验\n回收机：4点钓鱼经验\n种子生成器：4点耕种经验\n树液采集器：4点采集经验\n煤炭窑：4点采集经验\n熏鱼机：4点钓鱼经验\n重型熔炉：35点采矿经验");
        }

        if (e.Name.IsEquivalentTo("Data/Shops"))
        {
            e.Edit(asset =>
                {
                    var shopData = asset.AsDictionary<string, ShopData>().Data;
                    shopData["Dwarf"].Items.RemoveAll(itemData => itemData.ItemId is "(O)287" or "(O)288");
                    shopData["AdventureShop"].Items.RemoveAll(itemData => itemData.ItemId == "(O)441");
                }
            );
            Log.Info("移除商店中的炸弹、超级炸弹和爆炸弹丸");
        }

        if (e.Name.IsEquivalentTo("Data/CraftingRecipes"))
        {
            e.Edit(asset =>
                {
                    var craftingRecipes = asset.AsDictionary<string, string>().Data;
                    craftingRecipes["Cherry Bomb"] = "92 5 390 2//286/false/Mining 1/";
                    craftingRecipes["Bomb"] = "92 10 390 5//287/false/Mining 6/";
                    craftingRecipes["Mega Bomb"] = "92 20 390 10//288/false/Mining 8/";
                    craftingRecipes["Explosive Ammo"] = "92 10 390 10//441 5/false/Combat 8/";
                }
            );
            Log.Info("修改炸弹配方：\n5 树液 + 2 石头 = 1 樱桃炸弹\n10 树液 + 5 石头 = 1 炸弹\n20 树液 + 10 石头 = 1 超级炸弹\n10 树液 + 10 石头 = 5 爆炸弹丸");
        }
    }
}