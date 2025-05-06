using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.PiCore;
using weizinai.StardewValleyMod.PiCore.Extension;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.Menu;

internal class ServerCabinMenu : CarpenterMenu
{
    public ServerCabinMenu() : base("Robin")
    {
        this.Blueprints.Clear();
        var index = 0;
        foreach (var (id, data) in Game1.buildingData)
        {
            if (data.IndoorMapType == "StardewValley.Locations.Cabin")
            {
                this.Blueprints.Add(new BlueprintEntry(index++, id, data, null));
            }
        }
        this.SetNewActiveBlueprint(0);
    }

    public override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);

        if (this.onFarm)
        {
            var tile = PositionHelper.GetTilePositionFromMousePosition();
            var building = this.TargetLocation.getBuildingAt(new Vector2(tile.X, tile.Y))
                           ?? this.TargetLocation.getBuildingAt(new Vector2(tile.X, tile.Y + 1))
                           ?? this.TargetLocation.getBuildingAt(new Vector2(tile.X, tile.Y + 2))
                           ?? this.TargetLocation.getBuildingAt(new Vector2(tile.X, tile.Y + 3));
            if (building?.IsCabin(out _) == false) building.color = Color.White;
        }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (this.Action is CarpentryAction.Demolish or CarpentryAction.Paint)
        {
            var building = this.TargetLocation.getBuildingAt(PositionHelper.GetTilePositionFromMousePosition());
            if (building?.IsCabin(out _) == false) return;
        }

        base.receiveLeftClick(x, y, playSound);

        if (this.Action == CarpentryAction.Move && this.buildingToMove?.IsCabin(out _) == false)
        {
            this.buildingToMove.isMoving = false;
            this.buildingToMove = null;
        }
    }

    public override void update(GameTime time)
    {
        base.update(time);

        PanScreenHelper.PanScreen(32, 64);
    }
}