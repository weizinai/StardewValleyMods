using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Objects;
using weizinai.StardewValleyMod.Common;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;

namespace weizinai.StardewValleyMod.MiniMineShaft.Framework;

internal class MiniMine : GameLocation
{
    public static readonly List<MiniMine> ActiveMineShafts = new();

    private readonly LocalizedContentManager mapContent;

    private string? mapImageSource;

    public string? MapImageSource
    {
        get => this.mapImageSource;
        set
        {
            if (value != null && this.mapImageSource != value)
            {
                this.Map.RequireTileSheet(0, "mine").ImageSource = value;
                this.Map.LoadTileSheets(Game1.mapDisplayDevice);
            }
            this.mapImageSource = value;
        }
    }

    private Vector2 tileBeneathLadder;

    private MiniMine() : this(Game1.player.UniqueMultiplayerID) { }

    private MiniMine(long uniqueId)
    {
        this.name.Value = $"MiniMine_{uniqueId}";
        this.mapContent = Game1.game1.xTileContent.CreateTemporary();
    }

    public static bool IsMineName(string name)
    {
        return IsMineName(name, out _);
    }

    private static bool IsMineName(string name, out long uniqueId)
    {
        const string pattern = @"^MiniMine(_(?<UniqueID>\w+))?$";

        var match = Regex.Match(name, pattern);
        uniqueId = match.Groups["UniqueID"].Success ? long.Parse(match.Groups["UniqueID"].Value) : -1;

        return match.Success;
    }

    public static string GetMineName(long uniqueId)
    {
        return $"MiniMine_{uniqueId}";
    }

    public static MiniMine GetMine(string name)
    {
        if (!IsMineName(name, out var uniqueId))
        {
            Logger.Warn("TODO");
            return new MiniMine();
        }

        foreach (var mineShaft in ActiveMineShafts)
        {
            if (mineShaft.Name == name)
            {
                return mineShaft;
            }
        }

        var newMiniMineShaft = new MiniMine(uniqueId);
        newMiniMineShaft.GenerateContents();
        ActiveMineShafts.Add(newMiniMineShaft);

        return newMiniMineShaft;
    }

    public static void EnterMine(MiniMine mine)
    {
        Game1.warpFarmer(mine.Name, (int)mine.tileBeneathLadder.X, (int)mine.tileBeneathLadder.Y, 2);
    }

    public static void ClearActiveMines()
    {
        ActiveMineShafts.RemoveAll(mine =>
        {
            mine.OnRemoved();
            return true;
        });
    }

    public static void ClearInactiveMines()
    {
        ActiveMineShafts.RemoveAll(mine =>
        {
            if (mine.farmers.Any()) return false;
            mine.OnRemoved();
            return true;
        });
    }

    public static void UpdateMines(GameTime time)
    {
        foreach (var mineShaft in ActiveMineShafts)
        {
            if (mineShaft.farmers.Any())
            {
                mineShaft.UpdateWhenCurrentLocation(time);
            }
            mineShaft.updateEvenIfFarmerIsntHere(time);
        }
    }

    public static void UpdateMines10Minutes()
    {
        ClearInactiveMines();
    }

    private void GenerateContents()
    {
        this.LoadLevel();
        this.FindLadder();
    }

    private void LoadLevel()
    {
        var mapNumberToLoad = Game1.random.Next(40, 61);
        this.mapPath.Value = @"Maps\Mines\" + mapNumberToLoad;
        this.updateMap();

        this.MapImageSource = Game1.random.ChooseFrom(new[]
            {
                @"Maps\Mines\mine",
                @"Maps\Mines\mine_frost",
                @"Maps\Mines\mine_lava"
            }
        );
    }

    private void FindLadder()
    {
        var buildingLayer = this.map.RequireLayer("Buildings");

        for (var x = 0; x < buildingLayer.LayerWidth; x++)
        {
            for (var y = 0; y < buildingLayer.LayerHeight; y++)
            {
                var tileIndex = buildingLayer.GetTileIndexAt(x, y, "mine");
                if (tileIndex != -1)
                {
                    switch (tileIndex)
                    {
                        case 115:
                            this.tileBeneathLadder = new Vector2(x, y + 1);
                            break;
                    }
                }
            }
        }
    }

    public override bool AllowMapModificationsInResetState()
    {
        return true;
    }

    public override bool CanPlaceThisFurnitureHere(Furniture _furniture)
    {
        return false;
    }

    public override bool checkAction(Location tileLocation, Rectangle viewport, Farmer who)
    {
        if (who.IsLocalPlayer)
        {
            var index = this.getTileIndexAt(tileLocation, "Buildings", "mine");
            switch (index)
            {
                case 115:
                {
                    var options = new[]
                    {
                        new Response("Leave", Game1.content.LoadString("Strings\\Locations:Mines_LeaveMine")).SetHotKey(Keys.Y),
                        new Response("Do", Game1.content.LoadString("Strings\\Locations:Mines_DoNothing")).SetHotKey(Keys.Escape)
                    };
                    this.createQuestionDialogue(" ", options, "ExitMine");
                    return true;
                }
            }
        }

        return base.checkAction(tileLocation, viewport, who);
    }

    protected override LocalizedContentManager getMapLoader()
    {
        return this.mapContent;
    }

    public override void OnRemoved()
    {
        base.OnRemoved();
        this.mapContent.Unload();
    }

    protected override void resetLocalState()
    {
        base.resetLocalState();

        Logger.Info("TEST");
    }

    protected override void updateCharacters(GameTime time)
    {
        if (this.farmers.Any())
        {
            base.updateCharacters(time);
        }
    }
}