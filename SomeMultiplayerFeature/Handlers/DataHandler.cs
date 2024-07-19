using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Machines;
using StardewValley.GameData.Shops;
using StardewValley.GameData.WildTrees;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class DataHandler : BaseHandler
{
    public DataHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
    }

    public override void Clear()
    {
        this.Helper.Events.GameLoop.GameLaunched -= this.OnGameLaunched;
        this.Helper.Events.Content.AssetRequested -= this.OnAssetRequested;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        Log.Info("移除商店中的炸弹、超级炸弹和爆炸弹丸");
        Log.Info("修改树木生长的概率为50%、掉落种子的概率为0%");
        Log.Info("\n添加收获机器经验：\n小桶：20点耕种经验\n熔炉：7点采矿经验\n回收机：4点钓鱼经验\n种子生成器：4点耕种经验\n树液采集器：4点采集经验\n煤炭窑：4点采集经验\n熏鱼机：4点钓鱼经验\n重型熔炉：35点采矿经验");
        Log.Info("\n修改炸弹配方：\n2 树液 + 1 铜矿 = 1 樱桃炸弹\n4 树液 + 3 铁矿 = 1 炸弹\n8 树液 + 4 金矿 = 1 超级炸弹\n10 树液 + 10 铱矿 = 5 爆炸弹丸");
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        // 修改收获机器经验
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
        }

        // 移除商店中的炸弹、超级炸弹和爆炸弹丸
        if (e.Name.IsEquivalentTo("Data/Shops"))
        {
            e.Edit(asset =>
                {
                    var shopData = asset.AsDictionary<string, ShopData>().Data;
                    shopData["Dwarf"].Items.RemoveAll(itemData => itemData.ItemId is "(O)287" or "(O)288");
                    shopData["AdventureShop"].Items.RemoveAll(itemData => itemData.ItemId == "(O)441");
                }
            );
        }

        // 修改炸弹配方
        if (e.Name.IsEquivalentTo("Data/CraftingRecipes"))
        {
            e.Edit(asset =>
                {
                    var craftingRecipes = asset.AsDictionary<string, string>().Data;
                    craftingRecipes["Cherry Bomb"] = "92 2 378 1//286/false/Mining 1/";
                    craftingRecipes["Bomb"] = "92 3 380 3//287/false/Mining 6/";
                    craftingRecipes["Mega Bomb"] = "92 8 384 5//288/false/Mining 8/";
                    craftingRecipes["Explosive Ammo"] = "92 10 386 10//441 5/false/Combat 8/";
                }
            );
        }

        // 修改树木
        if (e.Name.IsEquivalentTo("Data/WildTrees"))
        {
            e.Edit(asset =>
            {
                var treeData = asset.AsDictionary<string, WildTreeData>().Data;
                foreach (var (id, data) in treeData)
                {
                    data.GrowthChance = 0.5f;
                    data.SeedSpreadChance = 0f;
                }
            });
        }
    }
}