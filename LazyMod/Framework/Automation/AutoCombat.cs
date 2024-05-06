using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;

namespace LazyMod.Framework.Automation;

public class AutoCombat : Automate
{
    private readonly ModConfig config;

    public AutoCombat(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        // 自动攻击怪物
        if (config.AutoAttackMonster && tool is MeleeWeapon weapon) AutoAttackMonster(location, player, weapon);
    }

    // 自动攻击怪物
    private void AutoAttackMonster(GameLocation location, Farmer player, MeleeWeapon weapon)
    {
        var grid = GetTileGrid(player, config.AutoAttackMonsterRange);
        foreach (var tile in grid)
        {
            var monsters = location.characters.OfType<Monster>().ToList();
            if (!monsters.Any()) return;

            foreach (var _ in monsters.Where(monster => monster.GetBoundingBox().Intersects(GetTileBoundingBox(tile))))
            {
                UseWeaponOnTile(location, player, weapon, tile);
            }
        }
    }

    private void UseWeaponOnTile(GameLocation location, Farmer player, MeleeWeapon weapon, Vector2 tile)
    {
        var attacked = location.damageMonster(
            areaOfEffect: GetTileBoundingBox(tile),
            minDamage: weapon.minDamage.Value,
            maxDamage: weapon.maxDamage.Value,
            isBomb: false,
            knockBackModifier: weapon.knockback.Value,
            addedPrecision: weapon.addedPrecision.Value,
            critChance: weapon.critChance.Value,
            critMultiplier: weapon.critMultiplier.Value,
            triggerMonsterInvincibleTimer: weapon.type.Value != MeleeWeapon.dagger,
            who: player
        );
        if (attacked) location.playSound(weapon.type.Value == MeleeWeapon.club ? "clubhit" : "daggerswipe", tile);
    }
}