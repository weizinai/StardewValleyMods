using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace LazyMod.Framework.Automation;

public class AutoScythe : Automate
{
    private readonly ModConfig config;

    public AutoScythe(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        if (config.AutoCleanDeadCrop && (tool is MeleeWeapon scythe && scythe.isScythe() || config.FindScytheFromInventory))
            AutoCleanDeadCrop(location, player);
    }

    // 自动清理枯萎作物
    private void AutoCleanDeadCrop(GameLocation location, Farmer player)
    {
        var scythe = FindToolFromInventory<MeleeWeapon>(true);
        if (scythe is null || !scythe.isScythe())
            return;

        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoHarvestCropRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: not null } hoeDirt)
            {
                var crop = hoeDirt.crop;
                if (crop.dead.Value) hoeDirt.destroyCrop(true);
            }
        }
    }
}