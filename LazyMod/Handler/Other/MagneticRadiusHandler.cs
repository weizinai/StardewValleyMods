using System;
using StardewValley;
using StardewValley.Buffs;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class MagneticRadiusHandler : BaseAutomationHandler
{
    private const string UniqueBuffId = "weizinai.LazyMod";

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (this.Config.MagneticRadiusIncrease == 0)
        {
            player.buffs.Remove(UniqueBuffId);
            return;
        }

        player.buffs.AppliedBuffs.TryGetValue(UniqueBuffId, out var buff);
        if (buff is not { millisecondsDuration: > 5000 } || Math.Abs(buff.effects.MagneticRadius.Value - this.Config.MagneticRadiusIncrease) > 0.1f)
        {
            buff = new Buff(
                id: UniqueBuffId,
                source: "Lazy Mod",
                duration: 60000,
                effects: new BuffEffects
                {
                    MagneticRadius = { Value = this.Config.MagneticRadiusIncrease * 64 }
                }
            );
            player.applyBuff(buff);
        }
    }
}