using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Foraging;

internal class ForageHandler : BaseAutomationHandler
{
    public ForageHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        var grid = this.GetTileGrid(this.Config.AutoForage.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null && obj.IsSpawnedObject) this.CollectSpawnedObject(location, player, tile, obj);

            foreach (var terrainFeature in location.largeTerrainFeatures)
                if (terrainFeature is Bush bush && this.CanForageBerry(tile, bush))
                    bush.performUseAction(tile);
        }
    }
    
    private void CollectSpawnedObject(GameLocation location, Farmer player, Vector2 tile, SObject obj)
    {
        var oldQuality = obj.Quality;
        var random = Utility.CreateDaySaveRandom(tile.X, tile.Y * 777f);
        // 物品质量逻辑
        if (player.professions.Contains(16) && obj.isForage())
        {
            obj.Quality = 4;
        }
        else if (obj.isForage())
        {
            if (random.NextDouble() < player.ForagingLevel / 30f)
            {
                obj.Quality = 2;
            }
            else if (random.NextDouble() < player.ForagingLevel / 15f)
            {
                obj.Quality = 1;
            }
        }

        // 任务物品逻辑
        if (obj.questItem.Value && obj.questId.Value != null && obj.questId.Value != "0" && !player.hasQuest(obj.questId.Value)) return;

        if (player.couldInventoryAcceptThisItem(obj))
        {
            if (player.IsLocalPlayer)
            {
                location.localSound("pickUpItem");
                DelayedAction.playSoundAfterDelay("coin", 300);
            }

            player.animateOnce(279 + player.FacingDirection);
            if (!location.isFarmBuildingInterior())
            {
                if (obj.isForage())
                {
                    if (obj.SpecialVariable == 724519)
                    {
                        player.gainExperience(2, 2);
                        player.gainExperience(0, 3);
                    }
                    else
                    {
                        player.gainExperience(2, 7);
                    }
                }

                // 紫色短裤逻辑
                if (obj.ItemId.Equals("789") && location.Name.Equals("LewisBasement"))
                {
                    var bat = new Bat(Vector2.Zero, -789)
                    {
                        focusedOnFarmers = true
                    };
                    Game1.changeMusicTrack("none");
                    location.playSound("cursed_mannequin");
                    location.characters.Add(bat);
                }
            }
            else
            {
                player.gainExperience(0, 5);
            }

            player.addItemToInventoryBool(obj.getOne());
            Game1.stats.ItemsForaged++;
            if (player.professions.Contains(13) && random.NextDouble() < 0.2 && !obj.questItem.Value && player.couldInventoryAcceptThisItem(obj) &&
                !location.isFarmBuildingInterior())
            {
                player.addItemToInventoryBool(obj.getOne());
                player.gainExperience(2, 7);
            }

            location.objects.Remove(tile);
            return;
        }

        obj.Quality = oldQuality;
    }

    private bool CanForageBerry(Vector2 tile, Bush bush)
    {
        return (Game1.season is Season.Spring || Game1.season is Season.Fall) &&
               bush.getBoundingBox().Intersects(this.GetTileBoundingBox(tile)) &&
               bush.tileSheetOffset.Value == 1 &&
               bush.size.Value == Bush.mediumBush &&
               !bush.townBush.Value;
    }
}