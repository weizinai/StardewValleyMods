using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class ItemCheatHandler : BaseHandler
{
    public ItemCheatHandler(IModHelper helper, ModConfig config) 
        : base(helper, config)
    {
    }

    public override void Init()
    {
        Helper.ConsoleCommands.Add("inventory", "", AccessInventory);
    }

    private void AccessInventory(string command, string[] args)
    {
        if (!Context.IsMainPlayer) return;
        
        var farmer = Game1.getOnlineFarmers().FirstOrDefault(x => x.Name == args[0]);

        if (farmer is null)
        {
            Log.Info($"{args[0]}不存在，无法访问该玩家的背包。");
        }
        else
        {
            Log.Alert($"{farmer.Name}的背包中有：");
            foreach (var item in farmer.Items)
            {
                if (item is null) continue;
                Log.Info($"{item.Stack}\t{item.DisplayName}");
            }
        }
    }
}