using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Shops;
using StardewValley.GameData.WildTrees;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Handler;
using xTile.Dimensions;
using xTile.ObjectModel;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

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
        Logger.Info("移除商店中的炸弹、超级炸弹和爆炸弹丸");
        Logger.Info("修改树木生长的概率为50%、掉落种子的概率为0%");
        Logger.Info("添加砍树桩掉落炸弹功能");
        Logger.Info("\n修改炸弹配方：\n2 树液 + 1 铱矿 = 1 樱桃炸弹\n4 树液 + 2 铱矿 = 1 炸弹\n8 树液 + 4 铱矿 = 1 超级炸弹\n10 树液 + 10 铱矿 = 5 爆炸弹丸");
        Logger.Info("修改克林特的营业时间为6:00 - 2:00");
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
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
                    craftingRecipes["Cherry Bomb"] = "92 2 386 1//286/false/Mining 1/";
                    craftingRecipes["Bomb"] = "92 4 386 2//287/false/Mining 6/";
                    craftingRecipes["Mega Bomb"] = "92 8 386 4//288/false/Mining 8/";
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
                foreach (var (_, data) in treeData)
                {
                    data.GrowthChance = 0.5f;
                    data.SeedSpreadChance = 0f;
                    data.ChopItems?.Add(new WildTreeChopItemData
                    {
                        Id = "(O)287",
                        ItemId = "(O)287",
                        ForStump = true
                    });
                }
            });
        }

        // 修改克林特上班时间
        if (e.Name.IsEquivalentTo("Maps/Town"))
        {
            e.Edit(asset =>
            {
                var map = asset.AsMap().Data;
                var tile = map.RequireLayer("Buildings").PickTile(new Location(94 * 64, 81 * 64), Game1.viewport.Size);
                tile.Properties["Action"] = new PropertyValue("LockedDoorWarp 5 19 Blacksmith 600 2600");
            });
        }
    }
}