using System.Linq;
using StardewValley;
using StardewValley.Characters;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class GarbageCanHandler : BaseAutomationHandler
{
    public GarbageCanHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (this.CheckNPCNearTile(location, player) && this.Config.StopGarbageCanNearVillager) return;

        this.ForEachTile(this.Config.AutoGarbageCan.Range, tile =>
        {
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") == 78)
            {
                var action = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Action", "Buildings");
                if (action?.StartsWith("Garbage") == true) this.CheckTileAction(location, player, tile);
            }
            return true;
        });
    }

    /// <summary>
    /// 检测周围是否有NPC
    /// </summary>
    /// <returns>如果有,则返回true,否则返回false</returns>
    private bool CheckNPCNearTile(GameLocation location, Farmer player)
    {
        var tile = player.Tile;
        var npcs = Utility.GetNpcsWithinDistance(tile, 7, location).ToList();
        if (!npcs.Any()) return false;
        var horse = npcs.FirstOrDefault(npc => npc is Horse);
        return horse is null || npcs.Count != 1;
    }
}