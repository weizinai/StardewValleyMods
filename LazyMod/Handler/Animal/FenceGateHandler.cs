using System;
using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class FenceGateHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        this.ForEachTile(this.Config.AutoOpenFenceGate.Range + 2, tile =>
        {
            location.objects.TryGetValue(tile, out var obj);

            if (obj is not Fence fence || !fence.isGate.Value) return true;

            var distance = this.GetDistance(player.Tile, tile);
            if (distance <= this.Config.AutoOpenFenceGate.Range && fence.gatePosition.Value == 0)
            {
                fence.toggleGate(player, true);
            }
            else if (distance > this.Config.AutoOpenFenceGate.Range + 1 && fence.gatePosition.Value != 0)
            {
                fence.toggleGate(player, false);
            }

            return true;
        });
    }

    private int GetDistance(Vector2 origin, Vector2 tile)
    {
        return Math.Max(Math.Abs((int)(origin.X - tile.X)), Math.Abs((int)(origin.Y - tile.Y)));
    }
}