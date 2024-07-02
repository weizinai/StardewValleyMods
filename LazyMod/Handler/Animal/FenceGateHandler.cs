using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class FenceGateHandler : BaseAutomationHandler
{
    public FenceGateHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        var grid = this.GetTileGrid(this.Config.AutoOpenFenceGate.Range + 2);
        
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not Fence fence || !fence.isGate.Value) continue;

            var distance = this.GetDistance(player.Tile, tile);
            if (distance <= this.Config.AutoOpenFenceGate.Range && fence.gatePosition.Value == 0)
            {
                fence.toggleGate(player, true);
            }
            else if (distance > this.Config.AutoOpenFenceGate.Range + 1 && fence.gatePosition.Value != 0)
            {
                fence.toggleGate(player, false);
            }
        }
    }
    
    private int GetDistance(Vector2 origin, Vector2 tile)
    {
        return Math.Max(Math.Abs((int)(origin.X - tile.X)), Math.Abs((int)(origin.Y - tile.Y)));
    }
}