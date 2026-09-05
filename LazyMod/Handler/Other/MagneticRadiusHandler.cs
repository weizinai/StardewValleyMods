using System;
using StardewValley;
using StardewValley.Buffs;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class MagneticRadiusHandler : BaseAutomationHandler
{
    private const string UniqueBuffId = "weizinai.LazyMod";

    public MagneticRadiusHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (this.config.MagneticRadiusIncrease == 0)
        {
            player.buffs.Remove(UniqueBuffId);

            return;
        }

        player.buffs.AppliedBuffs.TryGetValue(UniqueBuffId, out var buff);

        if (buff is null || buff.millisecondsDuration <= 5000 || Math.Abs(buff.effects.MagneticRadius.Value - this.config.MagneticRadiusIncrease) > 0.1f)
        {
            buff = new Buff(
                id: UniqueBuffId,
                source: "Lazy Mod",
                duration: 60000,
                effects: new BuffEffects
                {
                    MagneticRadius = { Value = this.config.MagneticRadiusIncrease * 64 }
                });
            player.applyBuff(buff);
        }
    }
}
